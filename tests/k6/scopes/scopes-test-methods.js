import http from "k6/http";
import {
  TestUser1,
  TestUser2,
  TestAdmin1,
  TestMasterAdmin,
} from "../testUsers.js";
import { BASE_URL, testingParams } from "../utils.js";
import { check } from "k6";

let testScopeName;

export class ScopesTests {
  GetScopesUnauthorized() {
    const url = `${BASE_URL}/api/scopes`;
    const res = http.get(url, testingParams.User1);
    check(res, {
      "status is 403": (r) => r.status === 403,
    });
  }

  GetScopesAuthorized() {
    const url = `${BASE_URL}/api/scopes`;
    const res = http.get(url, testingParams.MasterAdmin);

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body is not empty": (r) => r.json().length > 0,
    });
  }

  CreateNewScope() {
    const url = `${BASE_URL}/api/scopes`;
    const payload = JSON.stringify({
      name: "test-scope-tmp",
      displayName: "Test Scope tmp",
      description: "Test Scope tmp",
      resources: [],
    });

    const res = http.post(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains scope name": (r) =>
        r.json().hasOwnProperty("name"),
    });

    testScopeName = res.json().name;

    const resGet = http.get(`${url}`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "scope has been created": (r) =>
        r.json().some((scope) => scope.name === testScopeName),
    });
  }

  UpdateScope() {
    const url = `${BASE_URL}/api/scopes/${testScopeName}`;
    const payload = JSON.stringify({
      displayName: "Test Scope tmp updated",
      description: "Test Scope tmp updated",
      resources: [],
    });

    const res = http.put(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
    });

    const resGet = http.get(`${BASE_URL}/api/scopes`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated scope name": (r) =>
        r
          .json()
          .some((scope) => scope.displayName === "Test Scope tmp updated"),
    });
  }

  DeleteScope() {
    const url = `${BASE_URL}/api/scopes/${testScopeName}`;

    const res = http.del(url, null, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
    });

    const resGet = http.get(`${BASE_URL}/api/scopes`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "scope has been deleted": (r) =>
        !r.json().some((scope) => scope.name === testScopeName),
    });
  }
}
