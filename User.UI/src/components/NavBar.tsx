import { NavLink } from "react-router-dom";

export default function NavBar() {
  return (
    <header className="navbar">
      <div className="navbar-inner">
        <span className="brand">User Directory</span>
        <nav>
          <NavLink to="/add">Add</NavLink>
          <NavLink to="/" end>
            List
          </NavLink>
        </nav>
      </div>
    </header>
  );
}
