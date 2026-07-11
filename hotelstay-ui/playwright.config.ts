import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  timeout: 60_000,
  expect: {
    timeout: 10_000
  },
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry'
  },
  webServer: [
    {
      command: 'dotnet run --project ../HotelStay.Api/HotelStay.Api.csproj --launch-profile http',
      url: 'http://localhost:5000',
      reuseExistingServer: true,
      timeout: 120_000
    },
    {
      command: 'npm start -- --host localhost --port 4200',
      url: 'http://localhost:4200',
      reuseExistingServer: true,
      timeout: 120_000
    }
  ],
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'], channel: 'chrome' }
    }
  ]
});