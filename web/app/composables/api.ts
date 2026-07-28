import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const LobbyInformation = z
  .object({
    playerUsernames: z.array(z.string()).nullable(),
    gameId: z.string().uuid().nullable(),
  })
  .partial();
const ChessPieceEnum = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
  z.literal(5),
]);
const PieceColor = z.union([z.literal(0), z.literal(1)]);
const ChessPiece = z
  .object({
    type: ChessPieceEnum,
    playerId: z.string().nullable(),
    color: PieceColor,
  })
  .partial();
const ChessGameState = z.object({
  board: z.array(ChessPiece).nullable(),
  yourTurn: z.boolean(),
});
const RegisterRequest = z.object({
  username: z.string().nullable(),
  email: z.string().nullable(),
  password: z.string().nullable(),
});
const LoginRequest = z.object({
  username: z.string().nullable(),
  password: z.string().nullable(),
});
const AccessTokenResponse = z.object({
  tokenType: z.string().nullish(),
  accessToken: z.string().nullable(),
  expiresIn: z.number().int(),
  refreshToken: z.string().nullable(),
});
const RefreshRequest = z.object({ refreshToken: z.string().nullable() });
const ResendConfirmationEmailRequest = z.object({
  email: z.string().nullable(),
});
const ForgotPasswordRequest = z.object({ email: z.string().nullable() });
const ResetPasswordRequest = z.object({
  email: z.string().nullable(),
  resetCode: z.string().nullable(),
  newPassword: z.string().nullable(),
});
const ClaimsIdentity: z.ZodType<ClaimsIdentity> = z.lazy(() =>
  z
    .object({
      authenticationType: z.string().nullable(),
      isAuthenticated: z.boolean(),
      actor: ClaimsIdentity,
      bootstrapContext: z.unknown().nullable(),
      claims: z.array(Claim).nullable(),
      label: z.string().nullable(),
      name: z.string().nullable(),
      nameClaimType: z.string().nullable(),
      roleClaimType: z.string().nullable(),
    })
    .partial()
);
const Claim: z.ZodType<Claim> = z.lazy(() =>
  z
    .object({
      issuer: z.string().nullable(),
      originalIssuer: z.string().nullable(),
      properties: z.record(z.string()).nullable(),
      subject: ClaimsIdentity,
      type: z.string().nullable(),
      value: z.string().nullable(),
      valueType: z.string().nullable(),
    })
    .partial()
);
const InfoResponse = z.object({
  email: z.string().nullable(),
  isEmailConfirmed: z.boolean(),
});
const InfoRequest = z
  .object({
    newEmail: z.string().nullable(),
    newPassword: z.string().nullable(),
    oldPassword: z.string().nullable(),
  })
  .partial();

export const schemas = {
  LobbyInformation,
  ChessPieceEnum,
  PieceColor,
  ChessPiece,
  ChessGameState,
  RegisterRequest,
  LoginRequest,
  AccessTokenResponse,
  RefreshRequest,
  ResendConfirmationEmailRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ClaimsIdentity,
  Claim,
  InfoResponse,
  InfoRequest,
};

const endpoints = makeApi([
  {
    method: "get",
    path: "/api/games/chess/game/:gameId/status",
    alias: "Chess_GetGameStatus",
    requestFormat: "json",
    parameters: [
      {
        name: "gameId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: ChessGameState,
  },
  {
    method: "get",
    path: "/api/games/chess/lobby/:lobbyId/status",
    alias: "Chess_GetLobbyStatus",
    requestFormat: "json",
    parameters: [
      {
        name: "lobbyId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: LobbyInformation,
  },
  {
    method: "post",
    path: "/api/games/chess/lobby/create",
    alias: "Chess_CreateLobby",
    requestFormat: "json",
    response: z.string().uuid(),
  },
  {
    method: "get",
    path: "/api/identity/confirmEmail",
    alias: "getApiidentityconfirmEmail",
    requestFormat: "json",
    parameters: [
      {
        name: "userId",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "code",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "changedEmail",
        type: "Query",
        schema: z.string().optional(),
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/identity/forgotPassword",
    alias: "ForgotPassword",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ email: z.string().nullable() }),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/identity/login",
    alias: "Login",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: LoginRequest,
      },
      {
        name: "useCookies",
        type: "Query",
        schema: z.boolean().optional(),
      },
      {
        name: "useSessionCookies",
        type: "Query",
        schema: z.boolean().optional(),
      },
    ],
    response: AccessTokenResponse,
  },
  {
    method: "get",
    path: "/api/identity/manage/info",
    alias: "getApiidentitymanageinfo",
    requestFormat: "json",
    parameters: [
      {
        name: "Claims",
        type: "Query",
        schema: z.array(Claim).optional(),
      },
      {
        name: "Identities",
        type: "Query",
        schema: z.array(ClaimsIdentity).optional(),
      },
      {
        name: "Identity.Name",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Identity.AuthenticationType",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Identity.IsAuthenticated",
        type: "Query",
        schema: z.boolean().optional(),
      },
    ],
    response: InfoResponse,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.void(),
      },
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/identity/manage/info",
    alias: "postApiidentitymanageinfo",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: InfoRequest,
      },
      {
        name: "Claims",
        type: "Query",
        schema: z.array(Claim).optional(),
      },
      {
        name: "Identities",
        type: "Query",
        schema: z.array(ClaimsIdentity).optional(),
      },
      {
        name: "Identity.Name",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Identity.AuthenticationType",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Identity.IsAuthenticated",
        type: "Query",
        schema: z.boolean().optional(),
      },
    ],
    response: InfoResponse,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.void(),
      },
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/identity/refresh",
    alias: "Refresh",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ refreshToken: z.string().nullable() }),
      },
    ],
    response: AccessTokenResponse,
  },
  {
    method: "post",
    path: "/api/identity/register",
    alias: "Register",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: RegisterRequest,
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/identity/resendConfirmationEmail",
    alias: "ResendConfirmationEmail",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ email: z.string().nullable() }),
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/identity/resetPassword",
    alias: "ResetPassword",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: ResetPasswordRequest,
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "get",
    path: "/ws",
    alias: "getWs",
    requestFormat: "json",
    response: z.void(),
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
