import { Navigate, Route, Routes } from "react-router-dom";
import NavBar from "./components/NavBar";
import AddPage from "./pages/AddPage";
import ListPage from "./pages/ListPage";

export default function App() {
  return (
    <>
      <NavBar />
      <main className="container">
        <Routes>
          <Route path="/" element={<ListPage />} />
          <Route path="/add" element={<AddPage />} />
          <Route path="/edit/:id" element={<AddPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </>
  );
}
