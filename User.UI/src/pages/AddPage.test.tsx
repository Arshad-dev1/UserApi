import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import * as apiUsers from "../api/users";
import { ToastProvider } from "../components/Toast";
import AddPage from "./AddPage";

const createUser = jest.spyOn(apiUsers, "createUser");

const renderPage = () =>
  render(
    <MemoryRouter initialEntries={["/add"]}>
      <ToastProvider>
        <Routes>
          <Route path="/" element={<div>List page</div>} />
          <Route path="/add" element={<AddPage />} />
        </Routes>
      </ToastProvider>
    </MemoryRouter>,
  );

async function fillValidForm() {
  await userEvent.type(screen.getByLabelText("Name"), "Asha Rao");
  await userEvent.type(screen.getByLabelText("Age"), "29");
  await userEvent.type(screen.getByLabelText("City"), "Bengaluru");
  await userEvent.type(screen.getByLabelText("State"), "Karnataka");
  await userEvent.type(screen.getByLabelText("Pincode"), "560001");
}

beforeEach(() => jest.mocked(createUser).mockReset());

describe("AddPage", () => {
  it("shows inline validation errors and does not call the API", async () => {
    renderPage();
    await userEvent.click(screen.getByRole("button", { name: "Add user" }));

    expect(screen.getByText("Name is required.")).toBeInTheDocument();
    expect(screen.getByText("Age is required.")).toBeInTheDocument();
    expect(screen.getByText("Pincode is required.")).toBeInTheDocument();
    expect(createUser).not.toHaveBeenCalled();
  });

  it("submits valid data, redirects to the list and shows a success toast", async () => {
    jest.mocked(createUser).mockResolvedValue({
      id: 1, name: "Asha Rao", age: 29, city: "Bengaluru", state: "Karnataka", pincode: "560001",
    });
    renderPage();

    await fillValidForm();
    await userEvent.click(screen.getByRole("button", { name: "Add user" }));

    expect(createUser).toHaveBeenCalledWith({
      name: "Asha Rao", age: 29, city: "Bengaluru", state: "Karnataka", pincode: "560001",
    });
    expect(await screen.findByText("List page")).toBeInTheDocument();
    expect(screen.getByText("User added successfully.")).toBeInTheDocument();
  });

  it("shows an error message when the API call fails", async () => {
    jest.mocked(createUser).mockRejectedValue(new Error("Cannot reach the server."));
    renderPage();

    await fillValidForm();
    await userEvent.click(screen.getByRole("button", { name: "Add user" }));

    expect(await screen.findByRole("alert")).toHaveTextContent("Cannot reach the server.");
  });
});
