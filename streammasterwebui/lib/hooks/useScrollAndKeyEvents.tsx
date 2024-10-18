import { useEffect, useState } from 'react';
import EventManager from './eventManager';

type EventType = 'scroll' | 'keyUp' | 'keyDown' | 'mouseWheel' | 'wheel';
type Direction = 'up' | 'down';
type ScrollState = 'moved' | 'blocked';

interface UseScrollAndKeyEventsResult {
  type: EventType | null;
  direction: Direction | null;
  state?: ScrollState;
  code?: string | null;
}

function useScrollAndKeyEvents(): UseScrollAndKeyEventsResult {
  const [eventData, setEventData] = useState<UseScrollAndKeyEventsResult>({ direction: null, type: null, code: null });
  const eventManager = EventManager.getInstance();

  const determineScrollState = (direction: Direction) => {
    const atTop = window.scrollY <= 0;
    const atBottom = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight;

    if (direction === 'up' && atTop) {
      return 'blocked';
    }
    if (direction === 'down' && atBottom) {
      return 'blocked';
    }
    return 'moved';
  };

  useEffect(() => {
    const handleKeyDown = (e: Event) => {
      const keyboardEvent = e as KeyboardEvent;
      if (keyboardEvent.code === 'Enter' || keyboardEvent.code === 'NumpadEnter') {
        const state = determineScrollState('down');
        setEventData({ code: keyboardEvent.code, direction: 'down', state, type: 'keyDown' });
      }
    };

    const handleKeyUp = (e: Event) => {
      const keyboardEvent = e as KeyboardEvent;
      if (keyboardEvent.code === 'Enter' || keyboardEvent.code === 'NumpadEnter') {
        setEventData((prevData) => ({
          ...prevData,
          code: null,
          type: 'keyUp'
        }));
      }
    };

    const handleMouseWheelEvent = (e: Event) => {
      const wheelEvent = e as WheelEvent;
      const direction = wheelEvent.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'mouseWheel', code: null });
    };

    const handleWheelEvent = (e: Event) => {
      const wheelEvent = e as WheelEvent;
      const direction = wheelEvent.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'wheel', code: null });
    };

    eventManager.addEventListener('keydown', handleKeyDown);
    eventManager.addEventListener('keyup', handleKeyUp);
    eventManager.addEventListener('wheel', handleWheelEvent);
    eventManager.addEventListener('mousewheel', handleMouseWheelEvent);

    return () => {
      eventManager.removeEventListener('keydown', handleKeyDown);
      eventManager.removeEventListener('keyup', handleKeyUp);
      eventManager.removeEventListener('wheel', handleWheelEvent);
      eventManager.removeEventListener('mousewheel', handleMouseWheelEvent);
    };
  }, [eventManager]);

  return eventData;
}

export default useScrollAndKeyEvents;
