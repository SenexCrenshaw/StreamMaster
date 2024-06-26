import ColorEditor from '@components/inputs/ColorEditor';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import { Logger } from '@lib/common/logger';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useStringValue } from '@lib/redux/hooks/stringValue';
import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';

import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo } from 'react';

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
  const { setStringValue } = useStringValue('epgName');

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
      },
      save: () => {
        onUpdated();
      }
    }),
    [defaultValues, onUpdated, originalEPGFileDto]
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

  if (epgFileDto !== undefined) {
    Logger.debug('EPGFileDialog', { isSaveEnabled, epgFileDto: epgFileDto.Url, originalEPGFileDto: originalEPGFileDto?.Url });
  }

  if (selectedFile === undefined) {
    return null;
  }
  return (
    <div className="w-12 px-2">
      <div className="flex gap-1">
        <div className="w-6">
          <StringEditor
            disableDebounce
            darkBackGround
            autoFocus
            label="NAME"
            value={epgFileDto?.Name}
            onChange={(e) => {
              updateStateAndRequest({ Name: e });
              setStringValue(e);
              // e !== undefined && setName(e);
            }}
            onSave={(e) => {}}
          />
        </div>
        <div className="w-1">
          <div className="sm-sourceorfiledialog-toggle">
            <div className="flex flex-column">
              <ColorEditor color={epgFileDto.Color} label="ID" onChange={(e) => updateStateAndRequest({ Color: e })} />
            </div>
          </div>
        </div>
        <div className="w-4">
          <NumberEditor
            disableDebounce
            darkBackGround
            label="AUTO UPDATE"
            // onSave={(e) => {
            //   e !== undefined && setHoursToUpdate(e);
            // }}
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
            // onSave={(e) => {
            //   e !== undefined && setEPGNumber(e);
            // }}
            onChange={(e) => updateStateAndRequest({ EPGNumber: e })}
            value={epgFileDto.EPGNumber}
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

      <div className="w-12">
        <div className="flex gap-1">
          <div className="w-4">
            <NumberEditor
              disableDebounce
              darkBackGround
              label="TIME SHIFT"
              // onSave={(e) => {
              //   e !== undefined && setTimeShift(e);
              // }}
              onChange={(e) => updateStateAndRequest({ TimeShift: e })}
              value={epgFileDto.TimeShift}
            />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg" />
    </div>
  );
});

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
