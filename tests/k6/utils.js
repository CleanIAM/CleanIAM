import {
  TestUser1,
  TestUser2,
  TestAdmin1,
  TestAdmin2,
  TestMasterAdmin,
} from "./testUsers.js";

export const BASE_URL = "https://localhost:5000"; // Replace with your target URL

export const testingParams = {
  User1: {
    headers: {
      Authorization: `Bearer ${TestUser1.accessToken}`,
    },
    timeout: 10000, // Set a timeout of 10 seconds
  },
  User2: {
    headers: {
      Authorization: `Bearer ${TestUser2.accessToken}`,
    },
    timeout: 10000, // Set a timeout of 10 seconds
  },
  Admin1: {
    headers: {
      Authorization: `Bearer ${TestAdmin1.accessToken}`,
    },
    timeout: 10000, // Set a timeout of 10 seconds
  },
  Admin2: {
    headers: {
      Authorization: `Bearer ${TestAdmin2.accessToken}`,
    },
    timeout: 10000, // Set a timeout of 10 seconds
  },
  MasterAdmin: {
    headers: {
      Authorization: `Bearer ${TestMasterAdmin.accessToken}`,
    },
    timeout: 10000, // Set a timeout of 10 seconds
  },
};
