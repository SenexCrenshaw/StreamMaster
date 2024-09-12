import ColorEditor from '@components/inputs/ColorEditor';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import SMFileUpload, { SMFileUploadRef } from '@components/sm/SMFileUpload';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { CreateEPGFile, UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';

import { CreateEPGFileRequest, EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useRef } from 'react';

export interface EPGFileDialogProperties {
  readonly onEPGChanged?: (epgFileDto: EPGFileDto) => void;
  readonly onHide?: (didUpload: boolean) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
  readonly selectedFile: EPGFileDto;
  readonly showUrlEditor?: boolean;
}

export interface EPGFileDialogRef {
  reset: () => void;
  save: () => void;
}

const EPGFileDialog = forwardRef<EPGFileDialogRef, EPGFileDialogProperties>(({ onEPGChanged, onSaveEnabled, selectedFile, showUrlEditor = false }, ref) => {
  const smFileUploadRef = useRef<SMFileUploadRef>(null);
  const defaultValues = useMemo(
    () =>
      ({
        Color: '',
        EPGNumber: 1,
        HoursToUpdate: 72,
        Name: '',
        TimeShift: 0
      } as EPGFileDto),
    []
  );

  const { code } = useScrollAndKeyEvents();
  const [epgFileDto, setEPGFileDto] = React.useState<EPGFileDto>(defaultValues);
  const [originalEPGFileDto, setOriginalEPGFileDto] = React.useState<EPGFileDto | undefined>(undefined);
  const [request, setRequest] = React.useState<UpdateEPGFileRequest>({} as UpdateEPGFileRequest);

  const onUpdated = useCallback(async () => {
    if (request.Id === undefined) {
      return;
    }

    try {
      await UpdateEPGFile(request);
    } catch (error) {
      console.error(error);
    }
  }, [request]);

  useImperativeHandle(
    ref,
    () => ({
      reset: () => {
        if (originalEPGFileDto) {
          setEPGFileDto({ ...originalEPGFileDto });
        } else {
          setEPGFileDto(defaultValues);
        }
        smFileUploadRef.current?.reset();
      },
      save: () => {
        if (showUrlEditor) {
          onUpdated();
        } else {
          smFileUploadRef.current?.save();
        }
      }
    }),
    [defaultValues, onUpdated, originalEPGFileDto, showUrlEditor]
  );

  const isSaveEnabled = useMemo(() => {
    if (epgFileDto === undefined) {
      return false;
    }

    let isChanged = Object.keys(defaultValues).some((key) => epgFileDto[key as keyof EPGFileDto] !== originalEPGFileDto?.[key as keyof EPGFileDto]);

    onSaveEnabled?.(isChanged);
    return isChanged;
  }, [epgFileDto, defaultValues, onSaveEnabled, originalEPGFileDto]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<EPGFileDto>) => {
      if (epgFileDto === undefined) {
        return;
      }
      const updatedEPGFileDto = { ...epgFileDto, ...updatedFields };
      const updatedRequest = { ...request, Id: updatedEPGFileDto.Id, ...updatedFields };

      setEPGFileDto(updatedEPGFileDto);
      setRequest(updatedRequest);

      onEPGChanged && onEPGChanged(updatedEPGFileDto);
    },
    [epgFileDto, request, onEPGChanged]
  );

  useEffect(() => {
    if (selectedFile && (!originalEPGFileDto || selectedFile.Id !== originalEPGFileDto.Id)) {
      setEPGFileDto(selectedFile);
      setOriginalEPGFileDto(selectedFile);
    }
  }, [selectedFile, originalEPGFileDto]);

  useEffect(() => {
    if (code === 'Enter' || code === 'NumpadEnter') {
      if (isSaveEnabled) {
        onUpdated();
      }
    }
  }, [code, isSaveEnabled, onUpdated]);
  const ReturnToParent = useCallback((didUpload?: boolean) => {}, []);
  const onCreateFromSource = useCallback(
    async (source: string) => {
      const request = {} as CreateEPGFileRequest;

      request.Name = epgFileDto.Name;
      request.UrlSource = source;
      request.Color = epgFileDto.Color;
      request.EPGNumber = epgFileDto.EPGNumber;
      request.HoursToUpdate = epgFileDto.HoursToUpdate;
      request.TimeShift = epgFileDto.TimeShift;

      await CreateEPGFile(request)
        .then(() => {})
        .catch((error) => {
          console.error('Error uploading EPG', error);
        })
        .finally(() => {
          ReturnToParent();
        });
    },
    [ReturnToParent, epgFileDto.Color, epgFileDto.EPGNumber, epgFileDto.HoursToUpdate, epgFileDto.Name, epgFileDto.TimeShift]
  );

  // if (epgFileDto !== undefined) {
  //   Logger.debug('EPGFileDialog', { isSaveEnabled, epgFileDto: epgFileDto.Url, originalEPGFileDto: originalEPGFileDto?.Url });
  // }

  if (selectedFile === undefined) {
    return null;
  }
  return (
    <div className="px-2">
      <div className="flex flex-wrap flex-row w-12">
        <div className="w-12">
          <SMFileUpload
            isM3U={false}
            onSaveEnabled={(enabled) => {
              onSaveEnabled?.(enabled);
            }}
            ref={smFileUploadRef}
            epgFileDto={epgFileDto}
            onCreateFromSource={onCreateFromSource}
            onUploadComplete={() => {
              ReturnToParent(true);
            }}
            onFileNameChanged={(e) => {
              if (!epgFileDto || epgFileDto.Name === '') updateStateAndRequest({ Name: e });
            }}
          />
        </div>
        <div className="w-6">
          <StringEditor
            disableDebounce
            darkBackGround
            autoFocus
            label="NAME"
            value={epgFileDto?.Name}
            onChange={(e) => {
              updateStateAndRequest({ Name: e });
            }}
            onSave={(e) => {}}
          />
        </div>
        <div className="w-1">
          <div className="sm-sourceorfiledialog-toggle">
            <div className="flex flex-column">
              <ColorEditor
                color={epgFileDto.Color}
                label="ID"
                onChange={(e) => {
                  updateStateAndRequest({ Color: e });
                }}
              />
            </div>
          </div>
        </div>
        <div className="w-4">
          <NumberEditor
            disableDebounce
            darkBackGround
            label="AUTO UPDATE"
            onChange={(e) => updateStateAndRequest({ HoursToUpdate: e })}
            suffix=" Hours"
            value={epgFileDto.HoursToUpdate}
          />
        </div>
        <div className="w-2">
          <NumberEditor
            disableDebounce
            darkBackGround
            showButtons
            label="EPG #"
            onChange={(e) => updateStateAndRequest({ EPGNumber: e })}
            value={epgFileDto.EPGNumber}
          />
        </div>
        <div className="w-4">
          <NumberEditor
            disableDebounce
            darkBackGround
            label="TIME SHIFT"
            onChange={(e) => updateStateAndRequest({ TimeShift: e })}
            value={epgFileDto.TimeShift}
          />
        </div>
      </div>

      <div className="layout-padding-bottom-lg" />
      {showUrlEditor === true && (
        <>
          <div className="w-12">
            <StringEditor disableDebounce darkBackGround label="URL" value={epgFileDto?.Url} onChange={(e) => updateStateAndRequest({ Url: e })} />
          </div>
          <div className="layout-padding-bottom-lg" />
        </>
      )}

      <div className="layout-padding-bottom-lg" />
    </div>
  );
});

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
