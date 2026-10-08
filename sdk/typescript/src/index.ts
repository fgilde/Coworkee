import createClient, { type Client } from "openapi-fetch";
import type { paths } from "./schema";

export type { paths, components } from "./schema";

/**
 * A typed MyApp API client. `token` returns a bearer token of the MyApp API;
 * leave it out when the browser sends the session cookie through the MyApp web app.
 */
export function createMyAppClient(baseUrl: string, token?: () => string | Promise<string>): Client<paths> {
  const client = createClient<paths>({ baseUrl });
  if (token) {
    client.use({
      async onRequest({ request }) {
        request.headers.set("Authorization", `Bearer ${await token()}`);
        return request;
      },
    });
  }

  return client;
}
