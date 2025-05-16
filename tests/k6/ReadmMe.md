# K6 tests

The CleanIAM project includes a set of K6 tests to ensure the functionality and performance of the system.

---

## Prerequisites

- Install [K6](https://k6.io/docs/getting-started/installation/)
- Seed the database with test data usign `CleanIAM.BdSeed` project.
- Ensure that the CleanIAM API is running locally or in a test environment.
- Update the `BASE_URL` in the `utils.js` file to point to the correct API endpoint.

## Running the Tests

1. Open a terminal and navigate to the `tests/k6` directory.
2. Run the K6 tests using the following command:

   ```bash
   k6 run <test_file.js>
   ```

   Replace `<test_file.js>` with the name of the test file you want to run.

   - To run all tests, use:

   ```bash
   k6 run all.js
   ```

## Tests Structure

Each slice of cleanIam project is separated into its own test directory. Each test directory contains the following files:

- `[slice_name]-test-all.js`: The main test file for the slice.
- `[slice_name]-test-methods.js`: A test file containing individual test cases for each method in the slice.
- `utils.js`: A utility file containing helper functions and constants used in the tests.
- `README.md`: A readme file providing an overview of the tests and how to run them.

## Configuration

The k6 configuration is located in the `all.js` file. This file contains the configuration for the K6 tests, including the number of virtual users, test duration, and other settings. You can modify these settings to suit your testing needs.
