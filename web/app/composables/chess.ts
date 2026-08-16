import { z } from "zod";

export function unpackBoard(
  packedBoard: z.infer<typeof schemas.PackedBoardState>,
): Array<Array<{ imageUrl: string } | null>> {
  let x = 0;
  let y = 0;
  const result: Array<Array<{ imageUrl: string } | null>> = [];
  for (const row of packedBoard.packedBoard.split("\n")) {
    for (const piece of row) {
      // x y are right here

      result[x] ??= [];
      const url = packedBoard.imageUrls[piece];
      if (!url) {
        result[x]![y] = null;
      } else {
        result[x]![y] = { imageUrl: url };
      }

      // increment x y
      y++;
    }
    y = 0;
    x++;
  }

  return result;
}
