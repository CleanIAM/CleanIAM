import { group } from "k6";
import { TenantsTests } from "./tenants-test-methods.js";
const tenants = new TenantsTests();

export default function () {
  group("All Tenants Tests", function () {
    group("Get Tenants Unauthorized", function () {
      tenants.GetTenantsUnauthorized();
    });

    group("Get Tenants Authorized", function () {
      tenants.GetTenantsAuthorized();
    });

    group("Create New Tenant", function () {
      tenants.CreateNewTenant();
    });

    group("Update Tenant", function () {
      tenants.UpdateTenant();
    });

    group("Delete Tenant", function () {
      tenants.DeleteTenant();
    });
  });
}
