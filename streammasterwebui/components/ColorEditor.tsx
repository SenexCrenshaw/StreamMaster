import Sketch from '@uiw/react-color-sketch';
import { Button } from 'primereact/button';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useRef } from 'react';

interface ColorEditorProperties {
  readonly id?: string;
  readonly color: string;
  onChange(event: string): void;
}
const ColorEditor = ({ color, id, onChange }: ColorEditorProperties) => {
  const op = useRef<OverlayPanel>(null);

  console.log(color);
  return (
    <>
      <Button
        className="border-0"
        onClick={(e) => op.current?.toggle(e)}
        style={{
          backgroundColor: color
        }}
      />
      <OverlayPanel showCloseIcon={false} dismissable={true} ref={op} onClick={(e) => op.current?.toggle(e)}>
        <Sketch
          style={{ marginLeft: 20 }}
          color={color}
          onChange={(color) => {
            onChange(color.hex);
          }}
        />
      </OverlayPanel>
    </>
  );
};

export default ColorEditor;
