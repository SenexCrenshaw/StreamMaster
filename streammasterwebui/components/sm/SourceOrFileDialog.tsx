import { isValidUrl } from '@lib/common/common';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { InputText } from 'primereact/inputtext';
import { ProgressBar } from 'primereact/progressbar';
import { ChangeEvent, useCallback, useMemo, useRef, useState } from 'react';
import SMButton from './SMButton';
interface SourceOrFileDialogProps {
  isM3U: boolean;
  onAdd: (source: string | null, file: File | null) => void;
  progress?: number;
}

const SourceOrFileDialog = ({ isM3U, onAdd, progress }: SourceOrFileDialogProps) => {
  const [source, setSource] = useState<string | null>('');
  const [file, setFile] = useState<File | null>(null);
  const inputFile = useRef<HTMLInputElement>(null);
  const { setStringValue, stringValue } = useStringValue(isM3U ? 'm3uName' : 'epgName');

  const clearInputFile = useCallback(() => {
    setFile(null);
    if (inputFile.current !== null) {
      inputFile.current.value = '';
      inputFile.current.files = null;
      setStringValue('');
    }
  }, [setStringValue]);

  const sourceValue = useMemo(() => {
    var value = file ? file.name : source ?? '';
    return value;
  }, [file, source]);

  const isSaveEnabled = useMemo(() => {
    if (file) {
      return true;
    }
    if (source == null) return false;

    return isValidUrl(source) && stringValue !== undefined && stringValue !== '';
  }, [file, source, stringValue]);

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
        className="sm-sourceorfiledialog-url"
        disabled={file !== null}
        placeholder="Source URL or File"
        value={sourceValue}
        onChange={(e) => {
          clearInputFile();
          setSource(e.target.value);
          setStringValue(e.target.value);
        }}
      />
    );
  }, [progress, file, sourceValue, clearInputFile, setStringValue]);

  const handleChange = useCallback(
    (event: ChangeEvent<HTMLInputElement>): void => {
      var selectedFile = event.target.files?.[0];
      if (selectedFile) {
        setSource(null);
        setFile(selectedFile);
        setStringValue(selectedFile.name);
      }
    },
    [setStringValue]
  );

  const addIcon = useMemo((): string => {
    return file ? 'pi pi-upload' : 'pi pi-plus';
  }, [file]);

  return (
    <div className="sm-sourceorfiledialog flex flex-row grid-nogutter justify-content-between align-items-center">
      <div className="p-inputgroup">
        <SMButton rounded={false} buttonClassName="icon-orange" iconFilled icon="pi-upload" onClick={() => inputFile?.current?.click()} tooltip="Local File" />
        {getProgressOrInput}
        <SMButton
          buttonClassName="icon-red"
          buttonDisabled={file === null}
          icon="pi-times"
          iconFilled
          rounded={false}
          onClick={() => {
            clearInputFile();
            setSource(null);
          }}
          tooltip="Clear Selection"
        />
        <SMButton
          rounded={false}
          buttonClassName="icon-green"
          buttonDisabled={!isSaveEnabled}
          icon={addIcon}
          iconFilled
          onClick={() => {
            onAdd(source, file);
          }}
          tooltip="Add M3U"
        />
      </div>

      <input title="upload" ref={inputFile} type="file" onChange={handleChange} hidden />
    </div>
  );
};

export default SourceOrFileDialog;
