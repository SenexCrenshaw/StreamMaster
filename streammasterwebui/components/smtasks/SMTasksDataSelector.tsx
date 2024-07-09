import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { SMTask } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';

import useGetSMTasks from '@lib/smAPI/SMTasks/useGetSMTasks';
import SMDataTable from '../smDataTable/SMDataTable';

interface SMTasksDataSelectorProps {
  readonly height?: string;
  readonly needsTimer?: boolean;
  readonly width?: string;
}

const SMTasksDataSelector = ({ needsTimer = true, width = '40vw', height = '40vh' }: SMTasksDataSelectorProps) => {
  const [timer, setTimer] = useState<number>(0);

  useEffect(() => {
    let interval = null;
    if (needsTimer === true) {
      interval = setInterval(() => {
        setTimer((prev) => prev + 1); // Increment timer to force re-render
      }, 100);
    }
    return () => {
      if (interval !== null) {
        clearInterval(interval);
      }
    };
  }, [needsTimer]);

  const isBefore2020 = (dateStr: string): boolean => {
    const givenDate = new Date(dateStr);
    const comparisonDate = new Date('2020-01-01T00:00:00.000Z');

    // Compare the timestamps
    return givenDate < comparisonDate;
  };

  const startTSTemplate = useCallback((smTask: SMTask) => {
    if (isBefore2020(smTask.StartTS)) {
      return <div />;
    }
    return <div>{formatJSONDateString(smTask.StartTS ?? '')}</div>;
  }, []);

  const stopTSTemplate = useCallback((smTask: SMTask) => {
    if (isBefore2020(smTask.StopTS)) {
      return <div />;
    }

    return <div>{formatJSONDateString(smTask.StopTS ?? '')}</div>;
  }, []);

  const elapsedTSTemplate = useCallback(
    (smTask: SMTask) => {
      return <div className="numeric-field">{getElapsedTimeString(smTask.StartTS, smTask.StopTS)}</div>;
    },
    // Needed to update the elapsed time
    // eslint-disable-next-line react-hooks/exhaustive-deps
    []
  );

  const isRunningTemplate = useCallback((smTask: SMTask) => {
    if (smTask.IsRunning) {
      return <div className="pi pi-spin pi-spinner" />;
    }

    if (isBefore2020(smTask.StopTS)) {
      return <div />;
    }

    return <div className="pi pi-check icon-green" />;
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        align: 'center',
        bodyTemplate: isRunningTemplate,
        field: 'IsRunning',
        header: 'Running',
        width: 20
      },
      {
        field: 'Command',
        filter: true,
        width: 60
      },
      {
        align: 'center',
        bodyTemplate: startTSTemplate,
        field: 'StartTS',
        header: 'Started',
        width: 52
      },
      {
        align: 'center',
        bodyTemplate: stopTSTemplate,
        field: 'StopTS',
        header: 'Stopped',
        width: 52
      },
      {
        align: 'right',
        bodyTemplate: elapsedTSTemplate,
        field: 'ElapsedTS',
        header: '(d hh:mm:ss:ms)',
        width: 40
      }
    ],
    [elapsedTSTemplate, isRunningTemplate, startTSTemplate, stopTSTemplate]
  );

  const rowClass = useCallback((task: unknown): string => {
    const smTask = task as SMTask;
    if (smTask.IsRunning === true) {
      return 'channel-row-selected';
    }

    return '';
  }, []);

  return (
    <SMDataTable
      columns={columns}
      emptyMessage="No Tasks"
      id="SMTasksDataSelector"
      noIsLoading
      noSourceHeader
      queryFilter={useGetSMTasks}
      rowClass={rowClass}
      style={{ height: height, width: width }}
    />
  );
};

export default memo(SMTasksDataSelector);
