import { Style, Avatar } from "@dicebear/core";
import lorelei from "@dicebear/styles/lorelei.json";

const style = new Style(lorelei);

export function useAvatar() {
  function avatarUrl(seed: string) {
    const avatar = new Avatar(style, { seed });
    return `data:image/svg+xml;utf8,${encodeURIComponent(avatar.toString())}`;
  }

  return { avatarUrl };
}
