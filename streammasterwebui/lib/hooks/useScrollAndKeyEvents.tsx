import { useEffect, useState } from 'react';

type EventType = 'scroll' | 'keyUp' | 'keyDown' | 'mouseWheel' | 'wheel';
type Direction = 'up' | 'down';
type ScrollState = 'moved' | 'blocked';

interface UseScrollAndKeyEventsResult {
  type: EventType | null;
  direction: Direction | null;
  state?: ScrollState;
}

function useScrollAndKeyEvents(): UseScrollAndKeyEventsResult {
  const [eventData, setEventData] = useState<UseScrollAndKeyEventsResult>({ direction: null, type: null });

  const determineScrollState = (direction: Direction) => {
    const atTop = window.scrollY <= 0;
    const atBottom = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight;

    if (direction === 'up' && atTop) {
      return 'blocked';
    } if (direction === 'down' && atBottom) {
      return 'blocked';
    }
    return 'moved';
  };

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.keyCode === 38) {
        // Up arrow key code
        const state = determineScrollState('up');
        setEventData({ direction: 'up', state, type: 'keyUp' });
      } else if (e.keyCode === 40) {
        // Down arrow key code
        console.log('down');
        const state = determineScrollState('down');
        setEventData({ direction: 'down', state, type: 'keyDown' });
      }
    };

    const handleMouseWheelEvent = (e: any) => {
      // Use `any` to handle both WheelEvent and MouseWheelEvent
      const direction = e.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'mouseWheel' });
    };

    const handleWheelEvent = (e: WheelEvent) => {
      const direction = e.deltaY > 0 ? 'down' : 'up';
      const state = determineScrollState(direction);
      setEventData({ direction, state, type: 'wheel' }); // Updated the type to 'wheel'
    };

    // const handleMouseWheelEvent = (e: WheelEvent) => {
    //   const direction = e.deltaY > 0 ? 'down' : 'up';
    //   const state = determineScrollState(direction);
    //   setEventData({ type: 'mousewheel', direction, state }); // Updated the type to 'mousewheel'
    // };

    window.addEventListener('keydown', handleKeyDown);
    window.addEventListener('wheel', handleWheelEvent);
    window.addEventListener('mousewheel', handleMouseWheelEvent); // Listening for the mousewheel event

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
      window.removeEventListener('wheel', handleWheelEvent);
      window.removeEventListener('mousewheel', handleMouseWheelEvent);
    };
  }, []);

  return eventData;
}

export default useScrollAndKeyEvents;
