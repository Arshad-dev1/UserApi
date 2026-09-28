import { NavLink } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { clearAccessToken } from "../api/users";

export default function NavBar() {
  const navigate = useNavigate();
  const signOut = () => { clearAccessToken(); navigate("/login", { replace: true }); };
  return (
    <header className="navbar">
      <div className="navbar-inner">
        <span className="brand">User Directory</span>
        <nav>
          <NavLink to="/add">Add</NavLink>
          <NavLink to="/" end>
            List
          </NavLink>
          <button type="button" className="btn btn-small" onClick={signOut}>Sign out</button>
        </nav>
      </div>
    </header>
  );
}
