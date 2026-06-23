import http from "k6/http";
import { sleep, check } from "k6";

const BASE_URL = "http://172.209.217.168:5241";
const TOKEN =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiNmExMjkyZDU4OTFkNWJlMTk1MjJjYjNmIiwidXNlcl9uYW1lIjoiQWRtaW4gTm8uMSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTc4MjE0OTk5OCwiZXhwIjoxNzgyMTUwMjk4LCJpYXQiOjE3ODIxNDk5OTgsImlzcyI6IkNTUlMiLCJhdWQiOiJDU1JTIn0.iRZ273Jvhw2qHR7qqaKLpRdxurNG1wmLyjTdYqgjL_U";
const headers = { Authorization: `Bearer ${TOKEN}` };

export const options = {
  stages: [
    { duration: "30s", target: 10 },
    { duration: "1m", target: 50 },
    { duration: "2m", target: 50 },
    { duration: "30s", target: 0 },
  ],
  thresholds: {
    http_req_failed: ["rate<0.05"],
    http_req_duration: ["p(95)<2000"],
  },
};

export default function () {
  const from = new Date().toISOString();
  const to = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(); // One week from now

  const response = http.get(`${BASE_URL}/api/showtimes?from=${from}&to=${to}`, {
    headers,
  });
  check(response, { "status 200": (r) => r.status === 200 });

  sleep(1);
}
