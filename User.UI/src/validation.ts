import type { NewUser } from "./api/users";

export interface FormValues {
  name: string;
  age: string;
  city: string;
  state: string;
  pincode: string;
}

export type FormErrors = Partial<Record<keyof FormValues, string>>;

export const emptyForm: FormValues = { name: "", age: "", city: "", state: "", pincode: "" };

/** Mirrors the API rules: name 2-100, age 0-120, city/state required, pincode 4-10. */
export function validate(v: FormValues): FormErrors {
  const errors: FormErrors = {};

  const name = v.name.trim();
  if (!name) errors.name = "Name is required.";
  else if (name.length < 2 || name.length > 100) errors.name = "Name must be 2–100 characters.";

  const age = v.age.trim();
  if (!age) errors.age = "Age is required.";
  else if (!/^\d+$/.test(age) || Number(age) > 120) errors.age = "Age must be a whole number from 0 to 120.";

  const city = v.city.trim();
  if (!city) errors.city = "City is required.";
  else if (city.length > 100) errors.city = "City must be at most 100 characters.";

  const state = v.state.trim();
  if (!state) errors.state = "State is required.";
  else if (state.length > 100) errors.state = "State must be at most 100 characters.";

  const pincode = v.pincode.trim();
  if (!pincode) errors.pincode = "Pincode is required.";
  else if (pincode.length < 4 || pincode.length > 10) errors.pincode = "Pincode must be 4–10 characters.";

  return errors;
}

export function toNewUser(v: FormValues): NewUser {
  return {
    name: v.name.trim(),
    age: Number(v.age),
    city: v.city.trim(),
    state: v.state.trim(),
    pincode: v.pincode.trim(),
  };
}
