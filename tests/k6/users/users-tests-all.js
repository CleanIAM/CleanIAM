import { group } from "k6";
import { UsersTests } from "./users-tests-methods.js";
const users = new UsersTests();

export default function () {
  group("All Users Tests", function () {
    group("Get Users Unauthorized", function () {
      users.GetUsersUnauthorized();
    });

    group("Get Users Authorized", function () {
      users.GetUsersAuthorized();
    });

    group("Get User By Id Unauthorized", function () {
      users.GetUserByIdUnauthorized();
    });

    group("List Of Users Is Tenanted", function () {
      users.ListOfUsersIsTenanted();
    });

    group("Create New User", function () {
      users.CreateNewUser();
    });

    group("updateUser", function () {
      users.UpdateUser();
    });

    group("Delete User", function () {
      users.DeleteUser();
    });
  });
}
