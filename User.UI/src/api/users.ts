declare const __API_URL__: string | undefined;

const API_URL = (typeof __API_URL__ === "undefined" ? "http://localhost:5000" : __API_URL__).replace(/\/$/, "");
export const AUTH_STATE_CHANGED = "user-directory-auth-state-changed";

export function clearAccessToken(): void {
  localStorage.removeItem("accessToken");
  window.dispatchEvent(new Event(AUTH_STATE_CHANGED));
}

export interface User {
  id: number;
  name: string;
  age: number;
  city: string;
  state: string;
  pincode: string;
}

export type NewUser = Omit<User, "id">;

/** Error thrown for any failed API call. `fieldErrors` keys are lower-cased property names. */
export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
    public fieldErrors?: Record<string, string>,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

async function toApiError(res: Response): Promise<ApiError> {
  let body: { title?: string; detail?: string; errors?: Record<string, string[]> } = {};
  try {
    body = await res.json();
  } catch {
    /* non-JSON error body */
  }

  // ASP.NET returns { errors: { Name: ["..."] } } for 400 validation failures.
  const fieldErrors: Record<string, string> = {};
  for (const [key, messages] of Object.entries(body.errors ?? {})) {
    if (messages?.length) fieldErrors[key.toLowerCase()] = messages[0];
  }

  if (res.status === 401) return new ApiError("You need to sign in to do that.", 401);
  if (Object.keys(fieldErrors).length) {
    return new ApiError("Please fix the highlighted fields.", res.status, fieldErrors);
  }
  return new ApiError(body.detail ?? body.title ?? `Request failed (${res.status}).`, res.status);
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  let res: Response;
  try {
    res = await fetch(`${API_URL}${path}`, {
      ...init,
      headers: { "Content-Type": "application/json", ...(localStorage.getItem("accessToken") ? { Authorization: `Bearer ${localStorage.getItem("accessToken")}` } : {}), ...init.headers },
    });
  } catch (err) {
    if (err instanceof DOMException && err.name === "AbortError") throw err;
    throw new ApiError("Cannot reach the server. Check your connection and try again.", 0);
  }
  if (!res.ok) throw await toApiError(res);
  return (await res.json()) as T;
}

export async function login(username: string, password: string): Promise<void> {
  const result = await request<{ accessToken: string }>("/api/auth/login", {
    method: "POST", body: JSON.stringify({ username, password }),
  });
  localStorage.setItem("accessToken", result.accessToken);
  window.dispatchEvent(new Event(AUTH_STATE_CHANGED));
}

export const getUsers = (signal?: AbortSignal) => request<User[]>("/api/users", { signal });

export const getUser = (id: number, signal?: AbortSignal) =>
  request<User>(`/api/users/${id}`, { signal });

export const createUser = (user: NewUser) =>
  request<User>('/api/users', { method: 'POST', body: JSON.stringify(user) });

export const updateUser = (id: number, user: NewUser) =>
  request<User>(`/api/users/${id}`, { method: 'PUT', body: JSON.stringify(user) });
