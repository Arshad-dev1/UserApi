import { useEffect, useMemo, useState, type ChangeEvent, type FormEvent } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { ApiError, createUser, getUser, updateUser, type User } from "../api/users";
import { useToast } from "../components/Toast";
import { emptyForm, toNewUser, validate, type FormErrors, type FormValues } from "../validation";

const FIELDS: { name: keyof FormValues; label: string; type?: string }[] = [
  { name: "name", label: "Name" },
  { name: "age", label: "Age", type: "number" },
  { name: "city", label: "City" },
  { name: "state", label: "State" },
  { name: "pincode", label: "Pincode" },
];

export default function AddPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { id } = useParams();
  const { showToast } = useToast();
  const editingUser = (location.state as { user?: User } | null)?.user;
  const isEdit = !!(id || editingUser);
  const editId = editingUser?.id ?? (id ? Number(id) : undefined);

  const [values, setValues] = useState<FormValues>(emptyForm);
  const [touched, setTouched] = useState<Partial<Record<keyof FormValues, boolean>>>({});
  const [submitted, setSubmitted] = useState(false);
  const [serverErrors, setServerErrors] = useState<FormErrors>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (editingUser) {
      setValues({
        name: editingUser.name,
        age: String(editingUser.age),
        city: editingUser.city,
        state: editingUser.state,
        pincode: editingUser.pincode,
      });
      return;
    }

    if (!id) return;

    const controller = new AbortController();
    getUser(Number(id), controller.signal)
      .then((user) => {
        setValues({
          name: user.name,
          age: String(user.age),
          city: user.city,
          state: user.state,
          pincode: user.pincode,
        });
      })
      .catch((err) => {
        if (controller.signal.aborted) return;
        setFormError(err instanceof Error ? err.message : "Something went wrong.");
      });

    return () => controller.abort();
  }, [editingUser, id]);

  const clientErrors = useMemo(() => validate(values), [values]);
  const errors: FormErrors = { ...serverErrors, ...clientErrors };
  const visible = (field: keyof FormValues) => (submitted || touched[field] ? errors[field] : undefined);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const field = e.target.name as keyof FormValues;
    setValues((v) => ({ ...v, [field]: e.target.value }));
    setServerErrors((prev) => {
      const next = { ...prev };
      delete next[field];
      return next;
    });
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    setFormError(null);
    if (Object.keys(clientErrors).length > 0) return;

    setSubmitting(true);
    try {
      if (isEdit && editId !== undefined) {
        await updateUser(editId, toNewUser(values));
        showToast("User updated successfully.");
      } else {
        await createUser(toNewUser(values));
        showToast("User added successfully.");
      }
      navigate("/");
    } catch (err) {
      if (err instanceof ApiError && err.fieldErrors) {
        const mapped: FormErrors = {};
        for (const f of FIELDS) if (err.fieldErrors[f.name]) mapped[f.name] = err.fieldErrors[f.name];
        setServerErrors(mapped);
      }
      setFormError(err instanceof Error ? err.message : "Something went wrong.");
      setSubmitting(false);
    }
  };

  return (
    <section>
      <h1>{isEdit ? "Edit user" : "Add user"}</h1>

      <form onSubmit={handleSubmit} noValidate className="card form">
        {formError && (
          <div className="alert alert-error" role="alert">
            {formError}
          </div>
        )}

        {FIELDS.map((f) => {
          const error = visible(f.name);
          return (
            <div className="field" key={f.name}>
              <label htmlFor={f.name}>{f.label}</label>
              <input
                id={f.name}
                name={f.name}
                type={f.type ?? "text"}
                min={f.type === "number" ? 0 : undefined}
                max={f.type === "number" ? 120 : undefined}
                value={values[f.name]}
                onChange={handleChange}
                onBlur={() => setTouched((t) => ({ ...t, [f.name]: true }))}
                aria-required="true"
                aria-invalid={error ? "true" : "false"}
                aria-describedby={error ? `${f.name}-error` : undefined}
              />
              {error && (
                <p id={`${f.name}-error`} className="error-text">
                  {error}
                </p>
              )}
            </div>
          );
        })}

        <button type="submit" className="btn btn-primary" disabled={submitting}>
          {submitting ? "Saving…" : isEdit ? "Update user" : "Add user"}
        </button>
      </form>
    </section>
  );
}
