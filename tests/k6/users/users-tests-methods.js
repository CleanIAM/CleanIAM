import http from "k6/http";
import { TestUser1, TestUser2, TestAdmin1 } from "../testUsers.js";
import { BASE_URL, testingParams } from "../utils.js";
import { check } from "k6";

let testUser3Id;

export class UsersTests {
  GetUsersUnauthorized() {
    const url = `${BASE_URL}/api/users`;
    const res = http.get(url, testingParams.User1);
    check(res, {
      "status is 403": (r) => r.status === 403,
    });
  }

  GetUsersAuthorized() {
    const url = `${BASE_URL}/api/users`;
    const res = http.get(url, testingParams.MasterAdmin);

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body is not empty": (r) => r.json().length > 0,
    });
  }

  GetUserByIdUnauthorized() {
    const url = `${BASE_URL}/api/users/${TestUser1.id}`;
    const res = http.get(url, testingParams.User1);
    check(res, {
      "status is 403": (r) => r.status === 403,
    });
  }

  ListOfUsersIsTenanted() {
    const url = `${BASE_URL}/api/users`;
    const res = http.get(url, testingParams.Admin1);

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body is not empty": (r) => r.body.length > 0,
      "response body contains tenanted users": (r) =>
        r.json().some((user) => user.id === TestUser1.id),
      "response body does not contain other tenant users": (r) =>
        !r.json().some((user) => user.id === TestUser2.id),
    });
  }

  CreateNewUser() {
    const url = `${BASE_URL}/api/users/invited`;
    const payload = JSON.stringify({
      email: "user3@test.com",
      firstName: "Test",
      lastName: "User",
      roles: ["User"],
    });

    const res = http.post(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
      timeout: 10000, // Set a timeout of 10 seconds
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains new user ID": (r) =>
        r.json().hasOwnProperty("id"),
    });

    const getUrl = `${BASE_URL}/api/users/${res.json().id}`;
    const resGet = http.get(getUrl, testingParams.Admin1);
    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains new user ID": (r) =>
        r.json().id === res.json().id,
      "response body contains new user email": (r) =>
        r.json().email === "user3@test.com",
      "response body contains new user first name": (r) =>
        r.json().firstName === "Test",
      "response body contains new user last name": (r) =>
        r.json().lastName === "User",
      "response body contains new user roles": (r) =>
        r.json().roles.includes("User"),
    });

    testUser3Id = res.json().id;
  }

  UpdateUser() {
    const url = `${BASE_URL}/api/users/${testUser3Id}`;
    const payload = JSON.stringify({
      firstName: "Updated",
      lastName: "User",
      roles: ["User", "Admin"],
    });
    const res = http.put(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
      timeout: 10000, // Set a timeout of 10 seconds
    });
    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated user ID": (r) =>
        r.json().hasOwnProperty("id"),
    });

    const resGet = http.get(url, testingParams.Admin1);
    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated first name": (r) =>
        r.json().firstName === "Updated",
      "response body contains updated last name": (r) =>
        r.json().lastName === "User",
      "response body contains updated roles": (r) =>
        r.json().roles.includes("User") && r.json().roles.includes("Admin"),
    });
  }

  DeleteUser() {
    const url = `${BASE_URL}/api/users/${testUser3Id}`;
    const res = http.del(url, null, {
      headers: {
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
      timeout: 10000, // Set a timeout of 10 seconds
    });
    check(res, {
      "status is 200": (r) => r.status === 200,
    });

    const resGet = http.get(url, testingParams.Admin1);
    check(resGet, {
      "status is 404": (r) => r.status === 404,
    });
  }
}
