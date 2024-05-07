import { isValidUrl } from '@lib/common/common';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { ProgressBar } from 'primereact/progressbar';
import { ChangeEvent, useCallback, useMemo, useRef, useState } from 'react';
interface SourceOrFileDialogProps {
  onAdd: (source: string | null, file: File | null) => void;
  onName: (name: string) => void;
  progress?: number;
}

const SourceOrFileDialog = ({ onAdd, onName, progress }: SourceOrFileDialogProps) => {
  const [source, setSource] = useState<string | null>('');
  const [file, setFile] = useState<File | null>(null);
  const inputFile = useRef<HTMLInputElement>(null);

  const clearInputFile = useCallback(() => {
    setFile(null);
    if (inputFile.current !== null) {
      inputFile.current.value = '';
      inputFile.current.files = null;
      onName('');
    }
  }, [onName]);

  const sourceValue = useMemo(() => {
    var value = file ? file.name : source ?? '';
    return value;
  }, [file, source]);

  const isSaveEnabled = useMemo(() => {
    if (file) {
      return true;
    }
    if (source === null) return false;

    return isValidUrl(source);
  }, [source, file]);

  const getProgressOrInput = useMemo(() => {
    if (progress !== undefined && progress > 0) {
      return (
        <div className="flex-1 border-x-none">
          <ProgressBar className="border-x-none" value={progress} />
        </div>
      );
    }
    return (
      <InputText
        className="sourceOrFileDialog-url"
        disabled={file !== null}
        placeholder="Source URL or File"
        value={sourceValue}
        onChange={(e) => {
          clearInputFile();
          setSource(e.target.value);
        }}
      />
    );
  }, [file, progress, sourceValue, clearInputFile]);

  const handleChange = useCallback(
    (event: ChangeEvent<HTMLInputElement>): void => {
      var selectedFile = event.target.files?.[0];
      if (selectedFile) {
        setSource(null);
        setFile(selectedFile);
        onName(selectedFile.name);
      }
    },
    [onName]
  );

  const addIcon = useMemo((): string => {
    return file ? 'pi pi-upload' : 'pi pi-plus';
  }, [file]);

  return (
    <div className="sourceOrFileDialog flex flex-row grid-nogutter justify-content-between align-items-center">
      <div className="p-inputgroup flex">
        <Button className="icon-orange-filled w-1" icon="pi pi-upload" onClick={() => inputFile?.current?.click()} />
        {getProgressOrInput}
        <Button
          className="icon-red-filled w-1"
          disabled={file === null}
          icon="pi pi-times"
          onClick={() => {
            clearInputFile();
            setSource(null);
          }}
          tooltip="Clear File"
        />
        <Button
          className="icon-green-filled w-1"
          disabled={!isSaveEnabled}
          icon={addIcon}
          onClick={() => {
            onAdd(source, file);
          }}
        />
      </div>

      <input title="upload" ref={inputFile} type="file" onChange={handleChange} hidden />
    </div>
  );
};

export default SourceOrFileDialog;
