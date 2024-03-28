import { FieldData, PagedResponse,  } from '@lib/smAPI/smapiTypes';
import { GetApiArgument, QueryHookResult } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearSMMessages, intSetSMMessagesIsLoading, updateSMMessages } from '@lib/smAPI/SMMessages/SMMessagesSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setSMMessagesField: (fieldData: FieldData) => void;
  refreshSMMessages: () => void;
  setSMMessagesIsLoading: (isLoading: boolean) => void;
}

const useSMMessages = (params?: GetApiArgument | undefined): Result => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.SMMessages.data[query]);
  const isLoading = useAppSelector((state) => state.SMMessages.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.SMMessages.isError[query] ?? false);
  const error = useAppSelector((state) => state.SMMessages.error[query] ?? '');

  const setSMMessagesField = (fieldData: FieldData): void => {
    dispatch(updateSMMessages({ fieldData: fieldData }));
  };

  const refreshSMMessages = (): void => {
    dispatch(clearSMMessages());
  };

  const setSMMessagesIsLoading = (isLoading: boolean): void => {
    dispatch(intSetSMMessagesIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshSMMessages, setSMMessagesField, setSMMessagesIsLoading };
};

export default useSMMessages;
