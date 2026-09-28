import { emptyForm, toNewUser, validate } from "./validation";

const valid = { name: "Asha Rao", age: "29", city: "Bengaluru", state: "Karnataka", pincode: "560001" };

describe("validate", () => {
  it("accepts a valid form", () => {
    expect(validate(valid)).toEqual({});
  });

  it("requires every field", () => {
    expect(Object.keys(validate(emptyForm)).sort()).toEqual(["age", "city", "name", "pincode", "state"]);
  });

  it("enforces name length 2-100", () => {
    expect(validate({ ...valid, name: "A" }).name).toBeDefined();
    expect(validate({ ...valid, name: "A".repeat(101) }).name).toBeDefined();
    expect(validate({ ...valid, name: "Al" }).name).toBeUndefined();
  });

  it("enforces age 0-120 and integers only", () => {
    expect(validate({ ...valid, age: "0" }).age).toBeUndefined();
    expect(validate({ ...valid, age: "120" }).age).toBeUndefined();
    expect(validate({ ...valid, age: "121" }).age).toBeDefined();
    expect(validate({ ...valid, age: "-1" }).age).toBeDefined();
    expect(validate({ ...valid, age: "3.5" }).age).toBeDefined();
  });

  it("enforces pincode length 4-10", () => {
    expect(validate({ ...valid, pincode: "123" }).pincode).toBeDefined();
    expect(validate({ ...valid, pincode: "12345678901" }).pincode).toBeDefined();
    expect(validate({ ...valid, pincode: "1234" }).pincode).toBeUndefined();
  });

  it("treats whitespace-only values as empty", () => {
    expect(validate({ ...valid, city: "   " }).city).toBeDefined();
  });
});

describe("toNewUser", () => {
  it("trims strings and converts age to a number", () => {
    expect(toNewUser({ ...valid, name: "  Asha Rao ", age: "29" })).toEqual({
      name: "Asha Rao",
      age: 29,
      city: "Bengaluru",
      state: "Karnataka",
      pincode: "560001",
    });
  });
});
