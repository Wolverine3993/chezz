export function extractApiErrors(error: unknown): string[] {
  const response = (error as { response?: { status?: number; data?: unknown } })
    ?.response;
  const data = response?.data;

  if (typeof data === "string" && data) return [data];

  if (data && typeof data === "object") {
    const record = data as Record<string, unknown>;
    const source =
      record.errors && typeof record.errors === "object"
        ? (record.errors as Record<string, unknown>)
        : record;

    const messages = collectArrayMessages(source);
    if (messages.length) return messages;

    if (typeof record.detail === "string" && record.detail) return [record.detail];
    if (typeof record.title === "string" && record.title) return [record.title];
  }

  const fallback = statusMessage(response?.status);
  if (fallback) return [fallback];

  if (error instanceof Error && error.message) return [error.message];
  return ["An unexpected error occurred."];
}

function collectArrayMessages(obj: Record<string, unknown>): string[] {
  const messages: string[] = [];
  for (const value of Object.values(obj)) {
    if (!Array.isArray(value)) continue;
    for (const item of value) {
      if (typeof item === "string" && item) messages.push(item);
    }
  }
  return messages;
}

function statusMessage(status?: number): string | undefined {
  switch (status) {
    case 400:
      return "The request was invalid.";
    case 401:
      return "Incorrect username, password, or registration code.";
    case 403:
      return "You don't have permission to do that.";
    case 404:
      return "Not found.";
    case 409:
      return "That conflicts with existing data.";
    case 500:
      return "Something went wrong on our end. Please try again.";
    default:
      return status ? `Request failed (status ${status}).` : undefined;
  }
}
