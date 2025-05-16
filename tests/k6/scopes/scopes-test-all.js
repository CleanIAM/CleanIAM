import { group } from "k6";

import { ScopesTests } from "./scopes-test-methods.js";
const scopes = new ScopesTests();

export default function () {
  group("All Scopes Tests", function () {
    group("Get Scopes Unauthorized", function () {
      scopes.GetScopesUnauthorized();
    });

    group("Get Scopes Authorized", function () {
      scopes.GetScopesAuthorized();
    });

    group("Create New Scope", function () {
      scopes.CreateNewScope();
    });

    group("Update Scope", function () {
      scopes.UpdateScope();
    });

    group("Delete Scope", function () {
      scopes.DeleteScope();
    });
  });
}
