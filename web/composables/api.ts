import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const WeatherForecast = z
  .object({
    date: z.string(),
    temperatureC: z.number().int(),
    temperatureF: z.number().int(),
    summary: z.string().nullable(),
  })
  .partial();

export const schemas = {
  WeatherForecast,
};

const endpoints = makeApi([
  {
    method: "get",
    path: "/api/WeatherForecast",
    alias: "GetWeatherForecast",
    requestFormat: "json",
    response: z.array(WeatherForecast),
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
