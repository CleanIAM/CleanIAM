import http from "k6/http";
import {
  TestUser1,
  TestUser2,
  TestAdmin1,
  TestMasterAdmin,
} from "../testUsers.js";
import { BASE_URL, testingParams } from "../utils.js";
import { check } from "k6";

let applicationId;

export class ApplicationsTests {
  GetApplicationsUnauthorized() {
    const url = `${BASE_URL}/api/applications`;
    const res = http.get(url, testingParams.User1);
    check(res, {
      "status is 403": (r) => r.status === 403,
    });
  }

  GetApplicationsAuthorized() {
    const url = `${BASE_URL}/api/applications`;
    const res = http.get(url, testingParams.Admin1);

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body is not empty": (r) => r.json().length > 0,
    });
  }

  CreateNewApplication() {
    const url = `${BASE_URL}/api/applications`;
    const payload = JSON.stringify({
      clientId: "test-application-tmp",
      displayName: "Test Application tmp",
      applicationType: "web",
      clientType: "public",
      redirectUris: ["https://example.com/callback"],
      postLogoutRedirectUris: ["https://example.com/logout"],
      allowedScopes: ["openid", "profile", "email"],
      consentType: "implicit",
    });

    const res = http.post(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
      "response body contains application id": (r) =>
        r.json().hasOwnProperty("id"),
    });

    applicationId = res.json().id;

    const resGet = http.get(url, testingParams.Admin1);
    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains application id": (r) =>
        r.json().some((app) => app.id === applicationId),
    });
  }

  UpdateApplication() {
    const url = `${BASE_URL}/api/applications/${applicationId}`;
    const payload = JSON.stringify({
      clientId: "test-application-tmp",
      displayName: "Test Application tmp updated",
      applicationType: "web",
      clientType: "public",
      redirectUris: ["https://example.com/callback"],
      postLogoutRedirectUris: ["https://example.com/logout"],
      allowedScopes: ["openid", "profile", "email"],
      consentType: "implicit",
    });

    const res = http.put(url, payload, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
    });

    const resGet = http.get(url, testingParams.Admin1);

    console.log(resGet);

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body contains updated application name": (r) =>
        r.json().displayName === "Test Application tmp updated",
    });
  }

  DeleteApplication() {
    const url = `${BASE_URL}/api/applications/${applicationId}`;

    const res = http.del(url, null, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${TestAdmin1.accessToken}`,
      },
    });

    check(res, {
      "status is 200": (r) => r.status === 200,
    });

    const resGet = http.get(
      `${BASE_URL}/api/applications`,
      testingParams.Admin1
    );

    check(resGet, {
      "status is 200": (r) => r.status === 200,
      "response body does not contain deleted application id": (r) =>
        !r.json().some((app) => app.id === applicationId),
    });
  }
}
