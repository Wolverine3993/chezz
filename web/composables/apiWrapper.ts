import { createApiClient } from "./api";

export const useAPI = useState("api", () => createApiClient("/", {}));