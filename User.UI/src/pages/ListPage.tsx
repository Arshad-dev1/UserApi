import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getUsers, type User } from "../api/users";
import Spinner from "../components/Spinner";

type State =
  | { status: "loading" }
  | { status: "error"; message: string }
  | { status: "success"; users: User[] };

export default function ListPage() {
  const navigate = useNavigate();
  const [state, setState] = useState<State>({ status: "loading" });
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    setState({ status: "loading" });

    getUsers(controller.signal)
      .then((users) => setState({ status: "success", users }))
      .catch((err) => {
        if (controller.signal.aborted) return;
        setState({
          status: "error",
          message: err instanceof Error ? err.message : "Failed to load users.",
        });
      });

    return () => controller.abort();
  }, [attempt]);

  return (
    <section>
      <h1>Users</h1>

      {state.status === "loading" && <Spinner />}

      {state.status === "error" && (
        <div className="alert alert-error" role="alert">
          <p>{state.message}</p>
          <button type="button" className="btn" onClick={() => setAttempt((a) => a + 1)}>
            Retry
          </button>
        </div>
      )}

      {state.status === "success" && state.users.length === 0 && (
        <div className="empty">
          <p>No users yet.</p>
          <Link to="/add" className="btn btn-primary">
            Add the first user
          </Link>
        </div>
      )}

      {state.status === "success" && state.users.length > 0 && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Age</th>
                <th>City</th>
                <th>State</th>
                <th>Pincode</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {state.users.map((u) => (
                <tr key={u.id}>
                  <td>{u.name}</td>
                  <td>{u.age}</td>
                  <td>{u.city}</td>
                  <td>{u.state}</td>
                  <td>{u.pincode}</td>
                  <td>
                    <button
                      type="button"
                      className="btn btn-small"
                      onClick={() => navigate(`/edit/${u.id}`, { state: { user: u } })}
                    >
                      Edit
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
