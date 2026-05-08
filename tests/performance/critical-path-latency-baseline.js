import http from "k6/http";
import { Counter } from "k6/metrics";

let receivedResponse = new Counter("successfully_received_responses_count");
let timeout = new Counter("gateway_timeout_count");
let rateLimited = new Counter("rate_limited_count");

export const options = {
  stages: [
    { duration: "30s", target: 1000 },
    { duration: "30s", target: 1500 },
    { duration: "1m", target: 1500 },
    { duration: "30s", target: 0 },
  ],
};

export default function () {
  const res = http.get(
    "http://localhost:5241/api/seats?showtimeId=8d19433e-f36b-1410-87ae-00ffa1aa627f&available=true",
  );

  if (res.status === 200) receivedResponse.add(1);
  if (res.status === 400) timeout.add(1);
  if (res.status === 503) rateLimited.add(1);
}
