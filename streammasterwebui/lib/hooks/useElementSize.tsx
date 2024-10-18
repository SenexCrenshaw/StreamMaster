import { RefObject, useEffect, useState } from 'react';

interface Size {
  width: number | undefined;
  height: number | undefined;
}

function useElementSize<T extends HTMLElement = HTMLElement>(ref: RefObject<T>): Size {
  const [size, setSize] = useState<Size>({ width: undefined, height: undefined });

  useEffect(() => {
    const element = ref.current;

    if (!element) {
      return;
    }

    const resizeObserver = new ResizeObserver((entries) => {
      if (!Array.isArray(entries)) {
        return;
      }

      const entry = entries[0];
      setSize({
        width: entry.contentRect.width,
        height: entry.contentRect.height
      });
    });

    resizeObserver.observe(element);

    return () => {
      resizeObserver.disconnect();
    };
  }, [ref]);

  return size;
}

export default useElementSize;
