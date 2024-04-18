// eventManager.ts
type EventHandler = (e: Event) => void;

class EventManager {
  private static instance: EventManager;
  private handlers: Record<string, EventHandler[]> = {};

  private constructor() {}

  public static getInstance(): EventManager {
    if (!EventManager.instance) {
      EventManager.instance = new EventManager();
    }
    return EventManager.instance;
  }

  public addEventListener(eventType: string, handler: EventHandler): void {
    if (!this.handlers[eventType]) {
      this.handlers[eventType] = [];
      // Add the listener to the window object
      window.addEventListener(eventType, (e) => {
        this.handlers[eventType].forEach((h) => h(e));
      });
    }
    this.handlers[eventType].push(handler);
  }

  public removeEventListener(eventType: string, handler: EventHandler): void {
    const handlers = this.handlers[eventType];
    if (handlers) {
      const index = handlers.indexOf(handler);
      if (index !== -1) {
        handlers.splice(index, 1);
      }
    }
  }
}

export default EventManager;
