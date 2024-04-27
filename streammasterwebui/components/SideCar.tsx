import { Dropdown } from 'primereact/dropdown';
import React, { useCallback, useRef } from 'react';

interface SideCarProps {
  anchorRef: React.RefObject<Dropdown>;
  readonly children: React.ReactNode;
  readonly direction?: 'left' | 'right';
}

const SideCar: React.FC<SideCarProps> = ({ anchorRef, children, direction }) => {
  const popupRef = useRef<HTMLDivElement>(null);

  const calculatePopupPosition = useCallback(
    (popupRef2: React.RefObject<HTMLDivElement>) => {
      if (anchorRef.current && popupRef2.current) {
        const anchorRect = anchorRef.current.getElement().getBoundingClientRect();

        const popupRect = popupRef2.current.getBoundingClientRect();
        console.log('anchorRect', anchorRect);
        console.log('popupRect', popupRect);
        let height = anchorRect.height;
        const panel = document.querySelector('.sm-epg-editor-panel');
        if (panel) {
          var p = panel.getBoundingClientRect();
          height = p.height;
          console.log('panel', p);
        }

        if (direction === 'right') {
          return { height: height, left: popupRect.width * 2 };
        }

        return { height: height, left: -popupRect.width };
      }
      return { height: 0, left: 0 };
    },
    [anchorRef, direction]
  );

  return (
    <div
      className="w-6 surface-card sm-input-border sm-sidecar"
      ref={popupRef}
      style={{
        height: calculatePopupPosition(popupRef).height,
        left: calculatePopupPosition(popupRef).left,
        position: 'absolute',
        top: -2
      }}
    >
      {children}
    </div>
  );
};

export default SideCar;
