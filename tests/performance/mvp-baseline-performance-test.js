import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  stages: [
    { duration: "1m", target: 50 }, // Ramp up to 50 concurrent users
    { duration: "2m", target: 100 }, // Stress it with 100 users
    { duration: "1m", target: 0 }, // Ramp down
  ],
  thresholds: {
    http_req_duration: ["p(95)<1000"],
  },
};

export default function () {
  const res = http.get(
    "http://localhost:7019/api/seats?showtimeId=a1a0433e-f36b-1410-87ab-00ffa1aa627f&available=true",
  );

  check(res, {
    "status is 200": (r) => r.status === 200,
  });

  sleep(0.5);
}
