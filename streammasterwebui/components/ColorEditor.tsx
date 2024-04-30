import { getRandomColorHex } from '@lib/common/colors';
import Sketch from '@uiw/react-color-sketch';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useEffect, useRef, useState } from 'react';

interface ColorEditorProperties {
  readonly color: string;
  onChange?(event: string): void;
  readonly editable?: boolean;
}
const ColorEditor = ({ color: clientColor, editable, onChange }: ColorEditorProperties) => {
  const op = useRef<OverlayPanel>(null);
  const [color, setColor] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (clientColor !== undefined) {
      if (clientColor === '') {
        setColor(getRandomColorHex());
      } else if (color !== clientColor) {
        setColor(clientColor);
      }
    }
  }, [clientColor, color]);

  if (editable !== undefined && !editable) {
    return (
      <div
        className="color-editor-box flex justify-content-between align-items-center"
        style={{
          backgroundColor: clientColor
        }}
      />
    );
  }

  return (
    <>
      <div
        className="color-editor-box flex justify-content-between align-items-center"
        style={{
          backgroundColor: color
        }}
      >
        <i className="flex justify-content-center align-items-center sm-button pi pi-chevron-circle-down" onClick={(e) => op.current?.toggle(e)} />
        <i
          className="flex justify-content-center align-items-center sm-button pi pi-refresh"
          onClick={(e) => {
            const c = getRandomColorHex();
            setColor(c);
            onChange && onChange(c);
          }}
        />
      </div>
      <OverlayPanel showCloseIcon={false} dismissable={true} ref={op} onClick={(e) => op.current?.toggle(e)}>
        <Sketch
          style={{ marginLeft: 20 }}
          color={color}
          onChange={(color) => {
            setColor(color.hex);
            onChange && onChange(color.hex);
          }}
        />
      </OverlayPanel>
    </>
  );
};

export default ColorEditor;
