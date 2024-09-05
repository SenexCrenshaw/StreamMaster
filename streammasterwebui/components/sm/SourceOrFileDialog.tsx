import StringEditor from '@components/inputs/StringEditor';
import { isValidUrl } from '@lib/common/common';
import { ProgressBar } from 'primereact/progressbar';
import { ChangeEvent, forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import { SourceOrFileDialogProperties } from './Interfaces/SourceOrFileDialogProperties';
import SMButton from './SMButton';
import { SMFileUploadRef } from './SMFileUpload';
import { removeExtension } from '@lib/common/stringUtils';

interface ExtSourceOrFileDialogProperties extends SourceOrFileDialogProperties {
  onAdd: (source: string | null, file: File | null) => void;
  onFileNameChange: (fileName: string) => void;
}

const SourceOrFileDialog = forwardRef<SMFileUploadRef, ExtSourceOrFileDialogProperties>(
  ({ onAdd, onFileNameChange, onSaveEnabled, progress }: ExtSourceOrFileDialogProperties, ref) => {
    const [source, setSource] = useState<string | null>(null);
    const [file, setFile] = useState<File | null>(null);
    const inputFile = useRef<HTMLInputElement>(null);
    // const { setStringValue, stringValue } = useStringValue(isM3U ? 'm3uName' : 'epgName');

    const clearInputFile = useCallback(() => {
      setFile(null);
      if (inputFile.current !== null) {
        inputFile.current.value = '';
        inputFile.current.files = null;
      }
      // setStringValue(undefined);
      setSource(null);
    }, []);

    useImperativeHandle(
      ref,
      () => ({
        reset: () => {
          clearInputFile();
          setSource(null);
          // setStringValue(undefined);
        },
        save: () => {
          onAdd(source, file);
          // if (sourceOrFileDialogRef.current) {
          //   sourceOrFileDialogRef.current.save();
          // }
        }
      }),
      [clearInputFile, file, onAdd, source]
    );

    const sourceValue = useMemo(() => {
      var value = file ? file.name : source ?? '';
      return value;
    }, [file, source]);

    const isSaveEnabled = useMemo(() => {
      if (file) {
        return true;
      }
      if (source == null) return false;

      return isValidUrl(source);
    }, [file, source]);

    useEffect(() => {
      if (source !== null && source !== '') {
        onSaveEnabled(isSaveEnabled);
      }
    }, [isSaveEnabled, onSaveEnabled, source]);

    const getProgressOrInput = useMemo(() => {
      if (progress !== undefined && progress > 0) {
        return (
          <div className="flex-1 border-x-none">
            <ProgressBar className="border-x-none" value={progress} />
          </div>
        );
      }
      return (
        <div className="w-full pl-1">
          <StringEditor
            darkBackGround
            disabled={file !== null}
            disableDebounce
            onChange={(e) => {
              clearInputFile();
              if (e !== undefined) {
                setSource(e);
                // onFileNameChange?.(e);
              }
            }}
            placeholder="Source URL or File"
            showClear={false}
            value={sourceValue}
          />
        </div>
      );
    }, [progress, file, sourceValue, clearInputFile]);

    const handleChange = useCallback(
      (event: ChangeEvent<HTMLInputElement>): void => {
        var selectedFile = event.target.files?.[0];
        if (selectedFile) {
          setSource(null);
          setFile(selectedFile);
          onFileNameChange?.(removeExtension(selectedFile.name));
        }
      },
      [onFileNameChange]
    );

    return (
      <div className="sm-sourceorfiledialog flex flex-row grid-nogutter justify-content-between align-items-center">
        <div className="p-inputgroup">
          <SMButton buttonClassName="icon-orange" iconFilled icon="pi-upload" onClick={() => inputFile?.current?.click()} tooltip="Local File" />
          {getProgressOrInput}
        </div>

        <input title="upload" ref={inputFile} type="file" onChange={handleChange} hidden />
      </div>
    );
  }
);
export default SourceOrFileDialog;
