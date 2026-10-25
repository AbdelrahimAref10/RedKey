import { ApiException, ProblemDetails } from './redkey-api.generated';

const CODE_PATTERN = /^[a-z]+\.[a-zA-Z]+$/;

/**
 * Extracts the translatable error code sent by the API (ProblemDetails.detail
 * or the first validation error), e.g. `auth.invalidCredentials`.
 */
export function apiErrorCode(error: unknown): string {
  if (error instanceof ApiException) {
    if (error.status === 0) return 'network';

    const problem = (error.result ?? tryParse(error.response)) as ProblemDetails | undefined;
    const validation = problem?.['errors'] as Record<string, string[]> | undefined;
    const firstValidation = validation ? Object.values(validation).flat()[0] : undefined;

    if (firstValidation && CODE_PATTERN.test(firstValidation)) return firstValidation;
    if (problem?.detail && CODE_PATTERN.test(problem.detail)) return problem.detail;
    if (error.status >= 500) return 'server';
    return 'unknown';
  }

  // HttpErrorResponse with status 0 (backend down / offline) never reaches the generated client mapping.
  if (typeof error === 'object' && error !== null && 'status' in error && (error as { status: number }).status === 0) {
    return 'network';
  }

  return 'unknown';
}

function tryParse(text: string | undefined): unknown {
  if (!text) return undefined;
  try {
    return JSON.parse(text);
  } catch {
    return undefined;
  }
}
