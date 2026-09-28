import { useEffect, useState } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { AUTH_STATE_CHANGED } from "./api/users";
import NavBar from "./components/NavBar";
import AddPage from "./pages/AddPage";
import ListPage from "./pages/ListPage";
import LoginPage from "./pages/LoginPage";

export default function App() {
  const [authenticated, setAuthenticated] = useState(() => !!localStorage.getItem("accessToken"));

  useEffect(() => {
    const refreshAuthentication = () => setAuthenticated(!!localStorage.getItem("accessToken"));
    window.addEventListener(AUTH_STATE_CHANGED, refreshAuthentication);
    window.addEventListener("storage", refreshAuthentication);
    return () => {
      window.removeEventListener(AUTH_STATE_CHANGED, refreshAuthentication);
      window.removeEventListener("storage", refreshAuthentication);
    };
  }, []);

  if (!authenticated) return <main className="container"><Routes><Route path="/login" element={<LoginPage />} /><Route path="*" element={<Navigate to="/login" replace />} /></Routes></main>;
  return (
    <>
      <NavBar />
      <main className="container">
        <Routes>
          <Route path="/login" element={<Navigate to="/" replace />} />
          <Route path="/" element={<ListPage />} />
          <Route path="/add" element={<AddPage />} />
          <Route path="/edit/:id" element={<AddPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </>
  );
}
