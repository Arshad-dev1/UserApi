import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import * as apiUsers from "../api/users";
import ListPage from "./ListPage";

const getUsers = jest.spyOn(apiUsers, "getUsers");

const renderPage = () =>
  render(
    <MemoryRouter>
      <ListPage />
    </MemoryRouter>,
  );

beforeEach(() => jest.mocked(getUsers).mockReset());

describe("ListPage", () => {
  it("shows a spinner, then the users", async () => {
    jest.mocked(getUsers).mockResolvedValue([
      { id: 1, name: "Asha Rao", age: 29, city: "Bengaluru", state: "Karnataka", pincode: "560001" },
    ]);
    renderPage();

    expect(screen.getByRole("status")).toBeInTheDocument();
    expect(await screen.findByText("Asha Rao")).toBeInTheDocument();
    expect(screen.getByText("Bengaluru")).toBeInTheDocument();
    expect(screen.queryByRole("status")).not.toBeInTheDocument();
  });

  it("shows an empty state when there are no users", async () => {
    jest.mocked(getUsers).mockResolvedValue([]);
    renderPage();
    expect(await screen.findByText("No users yet.")).toBeInTheDocument();
  });

  it("shows an error and can retry", async () => {
    jest.mocked(getUsers).mockRejectedValueOnce(new Error("Boom")).mockResolvedValueOnce([]);
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent("Boom");
    await userEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("No users yet.")).toBeInTheDocument();
  });

  it("shows an edit button for each user", async () => {
    jest.mocked(getUsers).mockResolvedValue([
      { id: 1, name: "Asha Rao", age: 29, city: "Bengaluru", state: "Karnataka", pincode: "560001" },
      { id: 2, name: "Kiran S", age: 34, city: "Mysuru", state: "Karnataka", pincode: "570001" },
    ]);

    renderPage();

    expect(await screen.findAllByRole("button", { name: "Edit" })).toHaveLength(2);
  });
});
