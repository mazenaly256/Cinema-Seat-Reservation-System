import http from "k6/http";
import { Counter } from "k6/metrics";

let receivedResponse = new Counter("successfully_received_responses_count");
let timeout = new Counter("gateway_timeout_count");
let rateLimited = new Counter("rate_limited_count");
let auth = new Counter("auth");

export const options = {
  stages: [
    { duration: "30s", target: 1000 },
    { duration: "30s", target: 1500 },
    { duration: "1m", target: 1500 },
    { duration: "30s", target: 0 },
  ],
};

export default function () {
  const params = {
    headers: {
      Authorization:
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiNmExMjkzNjI4OTFkNWJlMTk1MjJjYjQwIiwidXNlcl9uYW1lIjoiQ2xpZW50IE5vLjEiLCJyb2xlIjoiQ2xpZW50IiwibmJmIjoxNzc5NjE1NDYxLCJleHAiOjE3Nzk2MTU3NjEsImlhdCI6MTc3OTYxNTQ2MSwiaXNzIjoiQ1NSUyIsImF1ZCI6IkNTUlMifQ.lzB_EfD9_Hil5gIfWzsEVaaGIFNmtZPLf-7SR_pe1J8",
    },
  };
  const res = http.get(
    "http://localhost:5241/api/seats?showtimeId=30000000-0000-0000-0000-000000000005&available=true",
  );

  if (res.status === 200) receivedResponse.add(1);
  else if (res.status === 400) timeout.add(1);
  else if (res.status === 503) rateLimited.add(1);
  else if (res.status === 401) auth.add(1);
  else timeout.add(1);
}
