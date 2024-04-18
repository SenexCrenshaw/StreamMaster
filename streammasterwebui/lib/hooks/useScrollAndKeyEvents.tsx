import { useEffect, useState } from 'react';
import EventManager from './eventManager';

type EventType = 'scroll' | 'keyUp' | 'keyDown' | 'mouseWheel' | 'wheel';
type Direction = 'up' | 'down';
type ScrollState = 'moved' | 'blocked';

interface UseScrollAndKeyEventsResult {
  type: EventType | null;
  direction: Direction | null;
  state?: ScrollState;
  code?: string;
}

function useScrollAndKeyEvents(): UseScrollAndKeyEventsResult {
  const [eventData, setEventData] = useState<UseScrollAndKeyEventsResult>({ direction: null, type: null });
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
    const handleKeyDown = (e: any) => {
      if (e.code === 'Enter' || e.code === 'NumpadEnter') {
        const state = determineScrollState('down');
        setEventData({ code: e.code, direction: 'down', state, type: 'keyDown' });
      }

      // const state = determineScrollState('down');
      // setEventData({ code: e.code, direction: 'down', state, type: 'keyUp' });
    };

    const handleMouseWheelEvent = (e: any) => {
      // Use `any` to handle both WheelEvent and MouseWheelEvent
      const direction = e.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'mouseWheel' });
    };

    const handleWheelEvent = (e: any) => {
      const direction = e.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'wheel' }); // Updated the type to 'wheel'
    };

    // const handleMouseWheelEvent = (e: WheelEvent) => {
    //   const direction = e.deltaY > 0 ? 'down' : 'up';
    //   const state = determineScrollState(direction);
    //   setEventData({ type: 'mousewheel', direction, state }); // Updated the type to 'mousewheel'
    // };

    eventManager.addEventListener('keyup', handleKeyDown);
    eventManager.addEventListener('wheel', handleWheelEvent);
    eventManager.addEventListener('mousewheel', handleMouseWheelEvent); // Listening for the mousewheel event

    return () => {
      eventManager.removeEventListener('keyup', handleKeyDown);
      eventManager.removeEventListener('wheel', handleWheelEvent);
      eventManager.removeEventListener('mousewheel', handleMouseWheelEvent);
    };
  }, []);

  return eventData;
}

export default useScrollAndKeyEvents;
