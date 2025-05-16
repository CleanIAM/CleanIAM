import http from "k6/http";
import {
  TestUser1,
  TestUser2,
  TestAdmin1,
  TestMasterAdmin,
} from "../testUsers.js";
import { BASE_URL, testingParams } from "../utils.js";
import { check } from "k6";

let testTenantId;

export class TenantsTests {
  GetTenantsUnauthorized() {
    const url = `${BASE_URL}/api/tenants`;
    const res = http.get(url, testingParams.User1);
    check(res, {
      "status is 403": (r) => r.status === 403,
    });
  }

  GetTenantsAuthorized() {
    const url = `${BASE_URL}/api/tenants`;
    const res = http.get(url, testingParams.MasterAdmin);

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body is not empty": (r) => r.json().length > 0,
    });
  }

  CreateNewTenant() {
    const url = `${BASE_URL}/api/tenants`;
    const payload = JSON.stringify({
      name: "Test Tenant tmp",
    });

    const res = http.post(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains tenant id": (r) => r.json().hasOwnProperty("id"),
    });

    testTenantId = res.json().id;

    const resGet = http.get(`${url}/${testTenantId}`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains tenant id": (r) => r.json().id === testTenantId,
    });
  }

  UpdateTenant() {
    const url = `${BASE_URL}/api/tenants/${testTenantId}`;
    const payload = JSON.stringify({
      name: "Updated Tenant",
    });

    const res = http.put(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated tenant name": (r) =>
        r.json().name === "Updated Tenant",
    });

    const resGet = http.get(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated tenant name": (r) =>
        r.json().name === "Updated Tenant",
      "response body contains tenant id": (r) => r.json().id === testTenantId,
    });
  }

  DeleteTenant() {
    const url = `${BASE_URL}/api/tenants/${testTenantId}`;

    const res = http.del(url, null, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(res, {
      "status is 204": (r) => r.status === 204,
    });

    const resGet = http.get(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
      },
    });

    check(resGet, {
      "status is 404": (r) => r.status === 404,
      "response body contains tenant not found message": (r) =>
        r.json().message === "Tenant not found",
    });
    testTenantId = null;
  }
}
