import StringEditor from '@components/inputs/StringEditor';
import { isValidUrl } from '@lib/common/common';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { ProgressBar } from 'primereact/progressbar';
import { ChangeEvent, forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import SMButton from './SMButton';
import { SMFileUploadRef } from './SMFileUpload';
import { SourceOrFileDialogProperties } from './interfaces/SourceOrFileDialogProperties';

interface ExtSourceOrFileDialogProperties extends SourceOrFileDialogProperties {
  onAdd: (source: string | null, file: File | null) => void;
}

const SourceOrFileDialog = forwardRef<SMFileUploadRef, ExtSourceOrFileDialogProperties>(
  ({ isM3U, onAdd, onSaveEnabled, progress }: ExtSourceOrFileDialogProperties, ref) => {
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
        setSource(null);
      }
    }, [setStringValue]);

    useImperativeHandle(
      ref,
      () => ({
        reset: () => {
          clearInputFile();
          setSource(null);
          setStringValue(undefined);
        },
        save: () => {
          onAdd(source, file);
          // if (sourceOrFileDialogRef.current) {
          //   sourceOrFileDialogRef.current.save();
          // }
        }
      }),
      [clearInputFile, file, onAdd, setStringValue, source]
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

      return isValidUrl(source) && stringValue !== undefined && stringValue !== '';
    }, [file, source, stringValue]);

    useEffect(() => {
      onSaveEnabled(isSaveEnabled);
    }, [isSaveEnabled, onSaveEnabled]);

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
            disableDebounce
            darkBackGround
            disabled={file !== null}
            showClear={false}
            placeholder="Source URL or File"
            value={sourceValue}
            onChange={(e) => {
              clearInputFile();
              if (e !== undefined) {
                setSource(e);
                setStringValue(e);
              }
            }}
          />
        </div>
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

    // const addIcon = useMemo((): string => {
    //   return file ? 'pi pi-upload' : 'pi pi-plus';
    // }, [file]);

    return (
      <div className="sm-sourceorfiledialog flex flex-row grid-nogutter justify-content-between align-items-center">
        <div className="p-inputgroup">
          <SMButton buttonClassName="icon-orange" iconFilled icon="pi-upload" onClick={() => inputFile?.current?.click()} tooltip="Local File" />
          {getProgressOrInput}
          {/* <SMButton
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
        /> */}
          {/* <SMButton
          rounded={false}
          buttonClassName="icon-green"
          buttonDisabled={!isSaveEnabled}
          icon={addIcon}
          iconFilled
          onClick={() => {
            onAdd(source, file);
          }}
          tooltip="Add M3U"
        /> */}
        </div>

        <input title="upload" ref={inputFile} type="file" onChange={handleChange} hidden />
      </div>
    );
  }
);
export default SourceOrFileDialog;
