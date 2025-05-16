import { group } from "k6";
import { ApplicationsTests } from "./applications-test-methods.js";
const applications = new ApplicationsTests();

export default function () {
  group("All Applications Tests", function () {
    group("Get Applications Unauthorized", function () {
      applications.GetApplicationsUnauthorized();
    });

    group("Get Applications Authorized", function () {
      applications.GetApplicationsAuthorized();
    });

    group("Create New Application", function () {
      applications.CreateNewApplication();
    });

    group("Update Application", function () {
      applications.UpdateApplication();
    });

    group("Delete Application", function () {
      applications.DeleteApplication();
    });
  });
}
