import { ColorPicker } from 'primereact/colorpicker';
import { useCallback } from 'react';
import { useDebouncedCallback } from 'use-debounce';

interface ColorEditorProperties {
  readonly color: string;
  onChange(event: string): void;
}
const ColorEditor = ({ color, onChange }: ColorEditorProperties) => {
  const debounced = useDebouncedCallback(
    useCallback(
      (value) => {
        if (!value.startsWith('#')) {
          value = '#' + value;
        }
        onChange(value);
      },
      [onChange]
    ),
    1500,
    {}
  );

  return <ColorPicker format="hex" value={color} onChange={(e) => debounced(e.value as string)} />;
};

export default ColorEditor;
