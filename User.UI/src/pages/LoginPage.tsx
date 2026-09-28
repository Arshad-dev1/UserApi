import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { ApiError, login } from "../api/users";

export default function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    setBusy(true);
    setError("");

    try {
      await login(username, password);
      navigate("/", { replace: true });
    } catch (reason) {
      setError(
        reason instanceof ApiError && reason.status === 401
          ? "Invalid username or password."
          : "Unable to sign in. Check the API and try again.",
      );
    } finally {
      setBusy(false);
    }
  };

  return (
    <section className="login-page">
      <h1>Sign in</h1>
      <form className="card form" onSubmit={submit}>
        {error && <div className="alert alert-error" role="alert">{error}</div>}

        <div className="field">
          <label htmlFor="username">Username</label>
          <input
            id="username"
            autoComplete="username"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            required
          />
        </div>

        <div className="field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />
        </div>

        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Signing in..." : "Sign in"}
        </button>
      </form>
    </section>
  );
}
