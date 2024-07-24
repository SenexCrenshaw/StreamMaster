import { GetVs } from '@lib/smAPI/Vs/VsCommands';
import { GetVsRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetVs = createAsyncThunk('cache/getGetVs', async (param: GetVsRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetVs');
    const response = await GetVs(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


