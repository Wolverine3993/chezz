import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const RelationshipRequest = z.object({ username: z.string() });
const PlayerMetadata = z.object({
  name: z.string(),
  elo: z.number().int(),
  imageUrl: z.string(),
});
const LobbyInformation = z.object({
  playerUsernames: z.array(z.string()),
  players: z.array(PlayerMetadata),
  gameId: z.string().uuid().nullish(),
  isPrivate: z.boolean(),
});
const PackedBoardState = z.object({
  packedBoard: z.string(),
  imageUrls: z.record(z.string()),
});
const PieceColor = z.enum(["White", "Black"]);
const ChessGameResult = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
]);
const ChessGameState = z.object({
  packedBoard: PackedBoardState,
  yourTurn: z.boolean(),
  yourColor: PieceColor,
  gameResult: ChessGameResult,
  moveHistory: z.array(z.string()),
  whitePlayer: PlayerMetadata,
  blackPlayer: PlayerMetadata,
});
const ChessPosition = z.object({ x: z.number().int(), y: z.number().int() });
const ChessMove = z.object({
  kind: z.string(),
  from: ChessPosition,
  to: ChessPosition,
  moveId: z.string(),
});
const NormalChessMove = ChessMove.and(
  z.object({ moveId: z.string(), kind: z.literal("Normal") })
);
const EnPassantChessMove = ChessMove.and(
  z.object({ moveId: z.string(), kind: z.literal("EnPassant") })
);
const CastlingChessMove = ChessMove.and(
  z.object({ moveId: z.string(), kind: z.literal("Castle") })
);
const ChessPieceEnum = z.enum([
  "Pawn",
  "Knight",
  "Bishop",
  "Rook",
  "Queen",
  "King",
]);
const ChessPiece = z.object({
  type: ChessPieceEnum,
  playerId: z.string(),
  color: PieceColor,
  imageUrl: z.string(),
});
const PromotionChessMove = ChessMove.and(
  z.object({
    promotionPiece: ChessPiece,
    moveId: z.string(),
    kind: z.literal("Promotion"),
  })
);
const ChessMoveChessPieceChessGameStateChessGameStoreChessGameImplementationMoveRequest =
  z.object({ moveId: z.string() });
const RegisterRequest = z.object({
  username: z.string(),
  email: z.string(),
  password: z.string(),
  registerCode: z.string(),
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
const ForgotPasswordRequest = z.object({ username: z.string() });
const ResetPasswordRequest = z.object({
  email: z.string(),
  resetCode: z.string(),
  newPassword: z.string(),
});
const InfoResponse = z.object({
  username: z.string(),
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
const NotificationType = z.union([z.literal(0), z.literal(1)]);
const Notification = z.object({
  id: z.string(),
  userId: z.string(),
  notificationType: NotificationType,
  title: z.string(),
  content: z.string(),
  callbackId: z.string(),
});
const FriendResponse = z.object({
  username: z.string().nullish(),
  id: z.string(),
});
const ChezzUser = z.object({
  id: z.string(),
  userName: z.string().nullish(),
  normalizedUserName: z.string().nullish(),
  email: z.string().nullish(),
  normalizedEmail: z.string().nullish(),
  emailConfirmed: z.boolean(),
  passwordHash: z.string().nullish(),
  securityStamp: z.string().nullish(),
  concurrencyStamp: z.string().nullish(),
  phoneNumber: z.string().nullish(),
  phoneNumberConfirmed: z.boolean(),
  twoFactorEnabled: z.boolean(),
  lockoutEnd: z.string().datetime({ offset: true }).nullish(),
  lockoutEnabled: z.boolean(),
  accessFailedCount: z.number().int(),
});
const FriendRequest = z.object({
  id: z.string(),
  userFromId: z.string(),
  userFrom: ChezzUser,
  userToId: z.string(),
  userTo: ChezzUser,
});
const FriendAcceptRequest = z.object({ requestId: z.string() });
const FriendDeclineRequest = z.object({ requestId: z.string() });

export const schemas = {
  RelationshipRequest,
  PlayerMetadata,
  LobbyInformation,
  PackedBoardState,
  PieceColor,
  ChessGameResult,
  ChessGameState,
  ChessPosition,
  ChessMove,
  NormalChessMove,
  EnPassantChessMove,
  CastlingChessMove,
  ChessPieceEnum,
  ChessPiece,
  PromotionChessMove,
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
  NotificationType,
  Notification,
  FriendResponse,
  ChezzUser,
  FriendRequest,
  FriendAcceptRequest,
  FriendDeclineRequest,
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
    response: z.array(
      z.union([
        NormalChessMove,
        EnPassantChessMove,
        CastlingChessMove,
        PromotionChessMove,
      ])
    ),
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
    method: "post",
    path: "/api/games/chess/lobby/:lobbyId/privacy",
    alias: "Chess_ChangeLobbyPrivacy",
    requestFormat: "json",
    parameters: [
      {
        name: "lobbyId",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "isPrivate",
        type: "Query",
        schema: z.boolean().optional(),
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/games/chess/lobby/:lobbyId/privacy",
    alias: "Chess_GetLobbyPrivacy",
    requestFormat: "json",
    parameters: [
      {
        name: "lobbyId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.boolean(),
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
    parameters: [
      {
        name: "isPrivate",
        type: "Query",
        schema: z.boolean().optional().default(true),
      },
    ],
    response: z.string().uuid(),
  },
  {
    method: "post",
    path: "/api/games/chess/lobby/matchmake",
    alias: "Chess_Matchmake",
    requestFormat: "json",
    response: z.string().uuid(),
  },
  {
    method: "post",
    path: "/api/games/chess/lobby/send-request",
    alias: "Chess_SendRequest",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ username: z.string() }),
      },
    ],
    response: z.string().uuid(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
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
        schema: z.object({ username: z.string() }),
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
    method: "post",
    path: "/api/identity/logout",
    alias: "Identity_Logout",
    requestFormat: "json",
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
  {
    method: "get",
    path: "/api/notificationList",
    alias: "Notification_GetNotifications",
    requestFormat: "json",
    response: Notification,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/relationship/accept-friend-request",
    alias: "UserRelationship_AcceptFriendRequest",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ requestId: z.string() }),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/relationship/add-friend",
    alias: "UserRelationship_AddFriendRequest",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ username: z.string() }),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
      {
        status: 409,
        description: `Conflict`,
        schema: z.string(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/relationship/decline-friend-request",
    alias: "UserRelationship_DeclineFriendRequest",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ requestId: z.string() }),
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/relationship/get-friend-requests",
    alias: "UserRelationship_GetFriendRequests",
    requestFormat: "json",
    response: z.array(FriendRequest),
  },
  {
    method: "get",
    path: "/api/relationship/get-friends",
    alias: "UserRelationship_GetFriends",
    requestFormat: "json",
    response: z.array(FriendResponse),
  },
  {
    method: "delete",
    path: "/api/relationship/remove-friend",
    alias: "UserRelationship_RemoveFriend",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ username: z.string() }),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.record(z.string()),
      },
    ],
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
