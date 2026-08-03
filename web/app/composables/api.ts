import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const LobbyInformation = z.object({
  playerUsernames: z.array(z.string()),
  gameId: z.string().uuid().nullish(),
});
const ChessPieceEnum = z.enum([
  "Pawn",
  "Knight",
  "Bishop",
  "Rook",
  "Queen",
  "King",
]);
const PieceColor = z.enum(["White", "Black"]);
const ChessPiece = z.object({
  type: ChessPieceEnum,
  playerId: z.string(),
  color: PieceColor,
  imageUrl: z.string(),
});
const ChessGameState = z.object({
  board: z.array(z.array(ChessPiece.nullable())),
  yourTurn: z.boolean(),
  yourColor: PieceColor,
});
const ChessPosition = z.object({ x: z.number().int(), y: z.number().int() });
const ChessMoveKind = z.enum(["Normal", "EnPassant", "Castle"]);
const ChessMove = z.object({
  from: ChessPosition,
  to: ChessPosition,
  kind: ChessMoveKind,
  moveId: z.string(),
});
const ChessMoveChessPieceChessGameStateChessGameStoreChessGameImplementationMoveRequest =
  z.object({ moveId: z.string() });
const RegisterRequest = z.object({
  username: z.string(),
  email: z.string(),
  password: z.string(),
});
const LoginRequest = z.object({ username: z.string(), password: z.string() });
const AccessTokenResponse = z.object({
  tokenType: z.string(),
  accessToken: z.string(),
  expiresIn: z.number().int(),
  refreshToken: z.string(),
});
const RefreshRequest = z.object({ refreshToken: z.string() });
const ResendConfirmationEmailRequest = z.object({ email: z.string() });
const ForgotPasswordRequest = z.object({ email: z.string() });
const ResetPasswordRequest = z.object({
  email: z.string(),
  resetCode: z.string(),
  newPassword: z.string(),
});
const InfoResponse = z.object({
  email: z.string(),
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
  ChessPosition,
  ChessMoveKind,
  ChessMove,
  ChessMoveChessPieceChessGameStateChessGameStoreChessGameImplementationMoveRequest,
  RegisterRequest,
  LoginRequest,
  AccessTokenResponse,
  RefreshRequest,
  ResendConfirmationEmailRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  InfoResponse,
  InfoRequest,
};

const endpoints = makeApi([
  {
    method: "post",
    path: "/api/games/chess/game/:gameId/move",
    alias: "Chess_MakeMove",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ moveId: z.string() }),
      },
      {
        name: "gameId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.boolean(),
  },
  {
    method: "get",
    path: "/api/games/chess/game/:gameId/moves",
    alias: "Chess_GetMoves",
    requestFormat: "json",
    parameters: [
      {
        name: "gameId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.array(ChessMove),
  },
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
    alias: "Identity_ConfirmEmail",
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
    alias: "Identity_ForgotPassword",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ email: z.string() }),
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
    alias: "Identity_Login",
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
    alias: "Identity_GetInfo",
    requestFormat: "json",
    response: InfoResponse,
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
    path: "/api/identity/manage/info",
    alias: "Identity_PostInfo",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: InfoRequest,
      },
    ],
    response: InfoResponse,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z.record(z.array(z.string())),
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
    alias: "Identity_Refresh",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ refreshToken: z.string() }),
      },
    ],
    response: AccessTokenResponse,
  },
  {
    method: "post",
    path: "/api/identity/register",
    alias: "Identity_Register",
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
        schema: z.record(z.array(z.string())),
      },
    ],
  },
  {
    method: "post",
    path: "/api/identity/resendConfirmationEmail",
    alias: "Identity_ResendConfirmationEmail",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ email: z.string() }),
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/identity/resetPassword",
    alias: "Identity_ResetPassword",
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
        schema: z.record(z.array(z.string())),
      },
    ],
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
