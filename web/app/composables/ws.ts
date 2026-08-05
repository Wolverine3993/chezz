export class WebsocketController {
  private send: string[] = [];
  private recieve: string[] = [];
  private listeners: Array<{
    callback: (message: string) => Promise<any>,
    queueIndex: number,
  }> = [];

  connected: Ref<boolean> = ref(false);

  constructor(endpoint: string) {
    const ws = new WebSocket(endpoint);

    ws.onopen = () => {
      for (const message of this.send) {
        ws.send(message);
      }
      this.connected.value = true;
    };

    ws.onmessage = (message) => {
      const msgString: string = message.data;
      this.recieve.push(msgString);
      for (let i = 0; i < this.listeners.length; i++) {
        this.reconcileListener(i);
      }
    };

    ws.onclose = (e) => {
        console.error(e);
    }
  }

  addListener(listener: (message: string) => Promise<any>) {
    this.listeners.push({ callback: listener, queueIndex: 0 });
    this.reconcileListener(this.listeners.length - 1);
    return () => this.removeListener(listener);
  }

  removeListener(listener: (message: string) => Promise<any>) {
    const index = this.listeners.findIndex((l) => l.callback === listener);
    if (index !== -1) this.listeners.splice(index, 1);
  }

  private async reconcileListener(index: number) {
    const queueMaxIndex = this.recieve.length;
    const listener = this.listeners[index];
    if(!listener) return;
    for(let i = listener.queueIndex; i < queueMaxIndex; i++) {
        await listener.callback(this.recieve[i]!);
    }
    this.listeners[index]!.queueIndex = queueMaxIndex;
  }
}

export const useWebsockets = () =>
  useState<{ [key: string]: WebsocketController }>("websockets", () => ({}));

export const createWebsocket = (endpoint: string) => {
  const wss = useWebsockets();
  if (wss.value[endpoint]) return wss.value[endpoint];

  wss.value[endpoint] = new WebsocketController(endpoint);
  return wss.value[endpoint];
};
