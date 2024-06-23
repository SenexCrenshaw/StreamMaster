import { QueryHookResult } from '@lib/apiDefs';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import store, { RootState } from '@lib/redux/store';
import { FieldData, SubscribedLineup } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect } from 'react';
import { fetchGetLineups } from './GetLineupsFetch';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetLineupsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<SubscribedLineup[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetLineups = (): Result => {
  const dispatch = useAppDispatch();
  const isForced = useAppSelector((state) => state.GetLineups.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({ tag: tag }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean): void => {
      dispatch(setIsLoading({ isLoading: isLoading }));
    },
    [dispatch]
  );
  const selectData = (state: RootState) => {
    return state.GetLineups.data;
  };
  const data = useAppSelector(selectData);

  const selectError = (state: RootState) => {
    return state.GetLineups.error;
  };
  const error = useAppSelector(selectError);

  const selectIsError = (state: RootState) => {
    return state.GetLineups.isError;
  };
  const isError = useAppSelector(selectIsError);

  const selectIsLoading = (state: RootState) => {
    return state.GetLineups.isLoading;
  };
  const isLoading = useAppSelector(selectIsLoading);

  useEffect(() => {
    const state = store.getState().GetLineups;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data]);

  useEffect(() => {
    const state = store.getState().GetLineups;
    if (state.isLoading) return;
    if (data !== undefined && !isForced) return;

    SetIsLoading(true);
    dispatch(fetchGetLineups());
  }, [SetIsLoading, data, dispatch, isForced]);

  const SetField = (fieldData: FieldData): void => {
    dispatch(setField({ fieldData: fieldData }));
  };

  const Clear = (): void => {
    dispatch(clear());
  };

  return {
    Clear,
    ClearByTag,
    data,
    error,
    isError,
    isLoading,
    SetField,
    SetIsForced,
    SetIsLoading
  };
};

export default useGetLineups;
