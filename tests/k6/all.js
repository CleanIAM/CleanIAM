import { group } from "k6";

import UsersTests from "./users/users-tests-all.js";
import TenantsTests from "./tenants/tenants-test-all.js";
import ScopesTests from "./scopes/scopes-test-all.js";
import ApplicationsTests from "./applications/applications-test-all.js";

export let options = {
  iterations: 1,
};
export default function () {
  group("All Tests", function () {
    UsersTests();
  });

  group("All Tenants Tests", function () {
    TenantsTests();
  });

  group("All Scopes Tests", function () {
    ScopesTests();
  });

  group("All Applications Tests", function () {
    ApplicationsTests();
  });
}
